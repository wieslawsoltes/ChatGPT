using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using AI;
using AI.Model.Json;
using AI.Model.Services;
using ChatGPT.Model.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Input;

namespace ChatGPT.ViewModels;

public class MainViewModel : ObservableObject
{
    private static readonly MainViewModelJsonContext s_serializerContext = new(
        new JsonSerializerOptions
        {
            WriteIndented = true,
            ReferenceHandler = ReferenceHandler.Preserve,
            IncludeFields = false,
            IgnoreReadOnlyFields = true,
            IgnoreReadOnlyProperties = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            NumberHandling = JsonNumberHandling.AllowNamedFloatingPointLiterals,
        });

    private ObservableCollection<ChatViewModel>? _chats;
    private ChatViewModel? _currentChat;
    private SettingsViewModel? _settings;
    private ActionsViewModel? _actions;
    private bool _isEnabled;

    public MainViewModel()
    {
        CreateDefaultActions();
        CreateDefaultSettings();

        _chats = new ObservableCollection<ChatViewModel>();
        _currentChat = new ChatViewModel
        {
            Name = "Chat",
            Settings = Settings?.ChatSettings ?? CreateDefaultChatSettings()
        };
        _chats.Add(_currentChat);

        _isEnabled = true;

        CreateWelcomeMessage();

        ChangeThemeCommand = new RelayCommand(ChangeThemeAction);
    }

    [JsonPropertyName("chats")]
    public ObservableCollection<ChatViewModel>? Chats
    {
        get => _chats;
        set => SetProperty(ref _chats, value);
    }

    [JsonPropertyName("currentChat")]
    public ChatViewModel? CurrentChat
    {
        get => _currentChat;
        set => SetProperty(ref _currentChat, value);
    }

    [JsonPropertyName("settings")]
    public SettingsViewModel? Settings
    {
        get => _settings;
        set => SetProperty(ref _settings, value);
    }

    [JsonPropertyName("isEnabled")]
    public bool IsEnabled
    {
        get => _isEnabled;
        set => SetProperty(ref _isEnabled, value);
    }

    [JsonIgnore]
    public IRelayCommand ChangeThemeCommand { get; }

    private void CreateDefaultActions()
    {
        _actions = new ActionsViewModel
        {
            New = NewAction,
            Open = OpenAction,
            Save = SaveAction,
            Export = ExportAction,
            Exit = () =>
            {
                var app = Ioc.Default.GetService<IApplicationService>();
                app?.Exit();
            }
        };
    }

    private void CreateDefaultSettings()
    {
        var settings = new SettingsViewModel
        {
            ChatSettings = CreateDefaultChatSettings()
        };
        settings.SetActions(_actions);
        Settings = settings;
    }

    private ChatSettingsViewModel CreateDefaultChatSettings()
    {
        return new ChatSettingsViewModel
        {
            Temperature = Defaults.DefaultTemperature,
            MaxTokens = Defaults.DefaultMaxTokens,
            Model = "gpt-3.5-turbo",
            ApiKey = null,
            Directions = Defaults.DefaultDirections,
            Format = Defaults.MarkdownMessageFormat,
        };
    }

    private void CreateWelcomeMessage()
    {
        if (CurrentChat is null)
        {
            return;
        }

        var welcomeItem = new ChatMessageViewModel
        {
            Prompt = "",
            Message = "Hi! I'm Clippy, your Windows Assistant. Would you like to get some assistance?",
            Format = Defaults.TextMessageFormat,
            IsSent = false,
            CanRemove = false
        };
        SetMessageActions(welcomeItem);
        CurrentChat.Messages.Add(welcomeItem);

        CurrentChat.CurrentMessage = welcomeItem;
    }

    private async Task NewAction()
    {
        NewCallback();
        await Task.Yield();
    }

    private async Task OpenAction()
    {
        var app = Ioc.Default.GetService<IApplicationService>();
        if (app is { })
        {
            await app.OpenFile(
                OpenCallbackAsync, 
                new List<string>(new[] { "Json", "All" }), 
                "Open");
        }
    }

    private async Task SaveAction()
    {
        var app = Ioc.Default.GetService<IApplicationService>();
        if (app is { })
        {
            await app.SaveFile(
                SaveCallbackAsync, 
                new List<string>(new[] { "Json", "All" }), 
                "Save", 
                "messages", 
                "json");
        }
    }

    private async Task ExportAction()
    {
        var app = Ioc.Default.GetService<IApplicationService>();
        if (app is { })
        {
            await app.SaveFile(
                ExportCallbackAsync, 
                new List<string>(new[] { "Text", "All" }), 
                "Export", 
                "messages", 
                "txt");
        }
    }

    private void ChangeThemeAction()
    {
        var app = Ioc.Default.GetService<IApplicationService>();
        if (app is { })
        {
            app.ToggleTheme();
        }
    }

    private void SetMessageActions(ChatMessageViewModel message)
    {
        message.SetSendAction(Send);
        message.SetCopyAction(Copy);
        message.SetRemoveAction(Remove);
    }

    private async Task Send(ChatMessageViewModel sendMessage)
    {
        if (CurrentChat is null 
            || Settings is null 
            || Settings.ChatSettings is null)
        {
            return;
        }

        if (string.IsNullOrEmpty(sendMessage.Prompt))
        {
            return;
        }

        var chatPrompt = CreateChatPrompt(sendMessage, CurrentChat.Messages, Settings.ChatSettings);

        IsEnabled = false;

        try
        {
            sendMessage.IsSent = true;

            ChatMessageViewModel? promptMessage;
            ChatMessageViewModel? resultMessage = null;

            if (sendMessage.Result is { })
            {
                promptMessage = sendMessage;
                resultMessage = sendMessage.Result;
            }
            else
            {
                promptMessage = new ChatMessageViewModel
                {
                    CanRemove = true,
                    Format = Defaults.TextMessageFormat
                };
                SetMessageActions(promptMessage);
                CurrentChat.Messages.Add(promptMessage);
            }

            var prompt = sendMessage.Prompt;

            promptMessage.Message = prompt;
            promptMessage.Prompt = "";
            promptMessage.IsSent = true;

            CurrentChat.CurrentMessage = promptMessage;
            promptMessage.IsAwaiting = true;

            // Response

            var chatServiceSettings = new ChatServiceSettings
            {
                Model = Settings.ChatSettings.Model,
                Messages = chatPrompt,
                Suffix = null,
                Temperature = Settings.ChatSettings.Temperature,
                MaxTokens = Settings.ChatSettings.MaxTokens,
                TopP = 1.0m,
                Stop = null,
            };
            var responseStr = default(string);
            var isResponseStrError = false;
            var responseData = await GetResponseData(chatServiceSettings, Settings.ChatSettings);
            if (responseData is null)
            {
                responseStr = "Unknown error.";
                isResponseStrError = true;
            }
            else if (responseData is ChatResponseError error)
            {
                var message = error.Error?.Message;
                responseStr = message ?? "Unknown error.";
                isResponseStrError = true;
            }
            else if (responseData is ChatResponseSuccess success)
            {
                var message = success.Choices?.FirstOrDefault()?.Message?.Content?.Trim();
                responseStr = message ?? "";
                isResponseStrError = false;
            }

            // Update

            if (isResponseStrError)
            {
                resultMessage = promptMessage;
            }

            if (resultMessage is null)
            {
                resultMessage = new ChatMessageViewModel
                {
                    IsSent = false,
                    CanRemove = true,
                    Format = Settings.ChatSettings.Format
                };
                SetMessageActions(resultMessage);
                CurrentChat.Messages.Add(resultMessage);
            }
            else
            {
                if (!isResponseStrError)
                {
                    resultMessage.IsSent = true;
                }
            }

            resultMessage.Message = responseStr;
            resultMessage.IsError = isResponseStrError;
            resultMessage.Prompt = isResponseStrError ? prompt : "";
            resultMessage.Format = Settings.ChatSettings.Format;

            if (CurrentChat.Messages.LastOrDefault() == resultMessage)
            {
                resultMessage.IsSent = false;
            }

            CurrentChat.CurrentMessage = resultMessage;

            promptMessage.IsAwaiting = false;
            promptMessage.Result = isResponseStrError ? null : resultMessage;
        }
        catch (Exception)
        {
            // ignored
        }

        IsEnabled = true;
    }

    private static ChatMessage[] CreateChatPrompt(
        ChatMessageViewModel sendMessage, 
        ObservableCollection<ChatMessageViewModel> messages, 
        ChatSettingsViewModel chatSettings)
    {
        var chatMessages = new List<ChatMessage>();

        chatMessages.Add(new ChatMessage
        {
            Role = "system",
            Content = chatSettings.Directions
        });

        // TODO: Ensure that chat prompt does not exceed maximum token limit.

        foreach (var message in messages)
        {
            if (!string.IsNullOrEmpty(message.Message) && message.Result is { })
            {
                chatMessages.Add(new ChatMessage
                {
                    Role = "user",
                    Content = message.Message
                });
                chatMessages.Add(new ChatMessage
                {
                    Role = "assistant",
                    Content = message.Result.Message
                });
            }
        }

        chatMessages.Add(new ChatMessage
        {
            Role = "user",
            Content = sendMessage.Prompt
        });

        return chatMessages.ToArray();
    }

    private static async Task<ChatResponse?> GetResponseData(ChatServiceSettings chatServiceSettings, ChatSettingsViewModel chatSettings)
    {
        var chat = Ioc.Default.GetService<IChatService>();
        if (chat is null)
        {
            return null;
        }

        var apiKey = Environment.GetEnvironmentVariable(Constants.EnvironmentVariableApiKey);
        var restoreApiKey = false;

        if (!string.IsNullOrWhiteSpace(chatSettings.ApiKey))
        {
            Environment.SetEnvironmentVariable(Constants.EnvironmentVariableApiKey, chatSettings.ApiKey);
            restoreApiKey = true;
        }

        ChatResponse? responseData = null;
        try
        {
            responseData = await chat.GetResponseDataAsync(chatServiceSettings);
        }
        catch (Exception)
        {
            // ignored
        }

        if (restoreApiKey && !string.IsNullOrWhiteSpace(apiKey))
        {
            Environment.SetEnvironmentVariable(Constants.EnvironmentVariableApiKey, apiKey);
        }

        return responseData;
    }

    private async Task Copy(ChatMessageViewModel message)
    {
        var app = Ioc.Default.GetService<IApplicationService>();
        if (app is { })
        {
            if (message.Message is { } text)
            {
                await app.SetClipboardText(text);
            }
        }
    }

    private void Remove(ChatMessageViewModel message)
    {
        if (CurrentChat is null)
        {
            return;
        }

        if (message is { CanRemove: true, IsAwaiting: false })
        {
            CurrentChat.Messages.Remove(message);

            var lastMessage = CurrentChat.Messages.LastOrDefault();
            if (lastMessage is { })
            {
                lastMessage.IsSent = false;
            }
        }
    }

    private void NewCallback()
    {
        if (CurrentChat is null)
        {
            return;
        }

        if (CurrentChat.Messages.Count <= 1)
        {
            CurrentChat.Messages[0].IsSent = false;
            return;
        }

        CurrentChat.Messages[0].Prompt = null;
        CurrentChat.Messages[0].IsSent = false;
        CurrentChat.Messages[0].Result = null;

        for (var i = CurrentChat.Messages.Count - 1; i >= 1; i--)
        {
            CurrentChat.Messages.RemoveAt(i);
        }
    }

    private async Task OpenCallbackAsync(Stream stream)
    {
        if (CurrentChat is null)
        {
            return;
        }

        var messages = await JsonSerializer.DeserializeAsync(
            stream, 
            s_serializerContext.ObservableCollectionChatMessageViewModel);
        if (messages is { })
        {
            NewCallback();

            if (CurrentChat.Messages.Count <= 1)
            {
                CurrentChat.Messages[0].IsSent = true;
            }

            for (var i = 0; i < messages.Count; i++)
            {
                var message = messages[i];

                if (i <= 0)
                {
                    continue;
                }

                SetMessageActions(message);
                CurrentChat.Messages.Add(message);
            }
        }
    }

    private async Task SaveCallbackAsync(Stream stream)
    {
        if (CurrentChat is null)
        {
            return;
        }

        await JsonSerializer.SerializeAsync(
            stream, 
            CurrentChat.Messages, s_serializerContext.ObservableCollectionChatMessageViewModel);
    }

    private async Task ExportCallbackAsync(Stream stream)
    {
        if (CurrentChat is null)
        {
            return;
        }

        await using var writer = new StreamWriter(stream);

        for (var i = 0; i < CurrentChat.Messages.Count; i++)
        {
            var message = CurrentChat.Messages[i];

            if (i < 1)
            {
                continue;
            }

            if (message.Result is { })
            {
                if (!string.IsNullOrEmpty(message.Message))
                {
                    await writer.WriteLineAsync(message.Message);
                    await writer.WriteLineAsync("");
                }

                if (!string.IsNullOrEmpty(message.Result.Message))
                {
                    await writer.WriteLineAsync(message.Result.Message);
                    await writer.WriteLineAsync("");
                }
            }
        }
    }

    public async Task LoadSettings(Stream stream)
    {
        var settings = await JsonSerializer.DeserializeAsync(
            stream, 
            s_serializerContext.SettingsViewModel);
        if (settings is { })
        {
            settings.SetActions(_actions);
            settings.ChatSettings ??= CreateDefaultChatSettings();
            Settings = settings;
        }
    }

    public async Task SaveSettings(Stream stream)
    {
        if (Settings is null)
        {
            return;
        }

        await JsonSerializer.SerializeAsync(
            stream, 
            Settings, s_serializerContext.SettingsViewModel);
    }
}
