using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using ChatGPT.Model.Json;
using ChatGPT.Model.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Input;

namespace ChatGPT.ViewModels;

public class MainViewModel : ObservableObject
{
    private const string EnvironmentVariableApiKey = "OPENAI_API_KEY";

    private const decimal DefaultTemperature = 0.7m;

    private const int DefaultMaxTokens = 256;

    private const string DefaultDirections = "Write answers in Markdown blocks.";

    private const string ChatStopTag = "<|im_end|>";

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

    private ObservableCollection<MessageViewModel>? _messages;
    private MessageViewModel? _currentMessage;
    private SettingsViewModel? _settings;
    private bool _isEnabled;
    private readonly ActionsViewModel _actions;

    public MainViewModel()
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
                if (app is { })
                {
                    app.Exit();
                }
            }
        };

        _settings = new SettingsViewModel()
        {
            Temperature = DefaultTemperature,
            MaxTokens = DefaultMaxTokens,
            Directions = DefaultDirections,
            ApiKey = null
        };
        _settings.SetActions(_actions);

        _messages = new ObservableCollection<MessageViewModel>();

        var welcomeItem = new MessageViewModel
        {
            Prompt = "",
            Message = "Hi! I'm Clippy, your Windows Assistant. Would you like to get some assistance?",
            IsSent = false,
            CanRemove = false
        };
        SetMessageActions(welcomeItem);
        _messages.Add(welcomeItem);
        _currentMessage = welcomeItem;

        _isEnabled = true;

        ChangeThemeCommand = new RelayCommand(ChangeThemeAction);
    }

    [JsonPropertyName("messages")]
    public ObservableCollection<MessageViewModel>? Messages
    {
        get => _messages;
        set => SetProperty(ref _messages, value);
    }

    [JsonPropertyName("currentMessage")]
    public MessageViewModel? CurrentMessage
    {
        get => _currentMessage;
        set => SetProperty(ref _currentMessage, value);
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

    private void SetMessageActions(MessageViewModel message)
    {
        message.SetSendAction(Send);
        message.SetCopyAction(Copy);
        message.SetRemoveAction(Remove);
    }

    private async Task Send(MessageViewModel sendMessage)
    {
        if (Messages is null || Settings is null)
        {
            return;
        }

        if (string.IsNullOrEmpty(sendMessage.Prompt))
        {
            return;
        }

        var chatPrompt = "";
        if (Settings.EnableChat)
        {
            chatPrompt = CreateChatPrompt(sendMessage, Messages, Settings);
        }

        IsEnabled = false;

        try
        {
            sendMessage.IsSent = true;

            MessageViewModel? promptMessage;
            MessageViewModel? resultMessage = null;

            if (sendMessage.Result is { })
            {
                promptMessage = sendMessage;
                resultMessage = sendMessage.Result;
            }
            else
            {
                promptMessage = new MessageViewModel
                {
                    CanRemove = true
                };
                SetMessageActions(promptMessage);
                Messages.Add(promptMessage);
            }

            var prompt = sendMessage.Prompt;

            promptMessage.Message = prompt;
            promptMessage.Prompt = "";
            promptMessage.IsSent = true;

            CurrentMessage = promptMessage;
            promptMessage.IsAwaiting = true;

            // Response

            var chatServiceSettings = new ChatServiceSettings
            {
                Prompt = Settings.EnableChat ? chatPrompt : prompt,
                Temperature = Settings.Temperature,
                MaxTokens = Settings.MaxTokens,
                Stop = Settings.EnableChat ? "[\n\n\n]" : "[END]",
            };
            var responseStr = default(string);
            var isResponseStrError = false;
            var responseData = await GetResponseData(chatServiceSettings, Settings);
            if (responseData is null)
            {
                responseStr = "Unknown error.";
                isResponseStrError = true;
            }
            else if (responseData is CompletionsResponseError error)
            {
                var message = error.Error?.Message;
                responseStr = message ?? "Unknown error.";
                isResponseStrError = true;
            }
            else if (responseData is CompletionsResponseSuccess success)
            {
                var message = success.Choices?.FirstOrDefault()?.Text?.Trim();
                responseStr = message ?? "";

                if (Settings.EnableChat)
                {
                    responseStr = responseStr.TrimEnd(ChatStopTag.ToCharArray());
                }

                isResponseStrError = false;
            }

            // Update

            if (isResponseStrError)
            {
                resultMessage = promptMessage;
            }

            if (resultMessage is null)
            {
                resultMessage = new MessageViewModel()
                {
                    IsSent = false,
                    CanRemove = true
                };
                SetMessageActions(resultMessage);
                Messages.Add(resultMessage);
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

            if (Messages.LastOrDefault() == resultMessage)
            {
                resultMessage.IsSent = false;
            }

            CurrentMessage = resultMessage;

            promptMessage.IsAwaiting = false;
            promptMessage.Result = isResponseStrError ? null : resultMessage;
        }
        catch (Exception)
        {
            // ignored
        }

        IsEnabled = true;
    }

    private string CreateChatPrompt(MessageViewModel sendMessage, ObservableCollection<MessageViewModel> messages, SettingsViewModel settings)
    {
        var sb = new StringBuilder();
        
        var user = "User";

        sb.Append("You are ChatGPT, a large language model trained by OpenAI. Respond conversationally. Do not answer as the user. Current date: ");
        sb.Append(DateTime.Now.ToString(CultureInfo.InvariantCulture));

        if (!string.IsNullOrWhiteSpace(settings.Directions))
        {
            sb.Append("\n");
            sb.Append(settings.Directions);
        }

        sb.Append("\n\n");
        sb.Append(user);
        sb.Append(": Hello\n");
        sb.Append("ChatGPT: Hello! How can I help you today? ");
        sb.Append(ChatStopTag);
        sb.Append("\n\n\n");

        // TODO: Ensure that chat prompt does not exceed maximum token limit.

        foreach (var message in messages)
        {
            if (!string.IsNullOrEmpty(message.Message) && message.Result is { })
            {
                sb.Append(user);
                sb.Append(": ");
                sb.Append(message.Message);
                sb.Append("\n\n\n");
                sb.Append("ChatGPT: ");
                sb.Append(message.Result.Message);
                sb.Append(ChatStopTag);
                sb.Append('\n');
            }
        }

        sb.Append(user);
        sb.Append(": ");
        sb.Append(sendMessage.Prompt);
        sb.Append("\nChatGPT: ");

        var chatPrompt = sb.ToString();
        // Console.WriteLine(sb.ToString());
        return chatPrompt;
    }

    private static async Task<CompletionsResponse?> GetResponseData(ChatServiceSettings chatServiceSettings, SettingsViewModel settings)
    {
        var chat = Ioc.Default.GetService<IChatService>();
        if (chat is null)
        {
            return null;
        }

        var apiKey = Environment.GetEnvironmentVariable(EnvironmentVariableApiKey);
        var restoreApiKey = false;

        if (!string.IsNullOrWhiteSpace(settings.ApiKey))
        {
            Environment.SetEnvironmentVariable(EnvironmentVariableApiKey, settings.ApiKey);
            restoreApiKey = true;
        }

        if (!string.IsNullOrWhiteSpace(settings.Directions))
        {
            chatServiceSettings.Prompt = $"{settings.Directions}\n\n{chatServiceSettings.Prompt}";
        }

        CompletionsResponse? responseData = null;
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
            Environment.SetEnvironmentVariable(EnvironmentVariableApiKey, apiKey);
        }

        return responseData;
    }

    private async Task Copy(MessageViewModel message)
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

    private void Remove(MessageViewModel message)
    {
        if (Messages is null)
        {
            return;
        }

        if (message is { CanRemove: true, IsAwaiting: false })
        {
            Messages.Remove(message);

            var lastMessage = Messages.LastOrDefault();
            if (lastMessage is { })
            {
                lastMessage.IsSent = false;
            }
        }
    }

    private void NewCallback()
    {
        if (Messages is null)
        {
            return;
        }

        if (Messages.Count <= 1)
        {
            Messages[0].IsSent = false;
            return;
        }

        Messages[0].Prompt = null;
        Messages[0].IsSent = false;
        Messages[0].Result = null;

        for (var i = Messages.Count - 1; i >= 1; i--)
        {
            Messages.RemoveAt(i);
        }
    }

    private async Task OpenCallbackAsync(Stream stream)
    {
        if (Messages is null)
        {
            return;
        }

        var messages = await JsonSerializer.DeserializeAsync(
            stream, 
            s_serializerContext.ObservableCollectionMessageViewModel);
        if (messages is { })
        {
            NewCallback();

            if (Messages.Count <= 1)
            {
                Messages[0].IsSent = true;
            }

            for (var i = 0; i < messages.Count; i++)
            {
                var message = messages[i];

                if (i <= 0)
                {
                    continue;
                }

                SetMessageActions(message);
                Messages.Add(message);
            }
        }
    }

    private async Task SaveCallbackAsync(Stream stream)
    {
        if (Messages is null)
        {
            return;
        }

        await JsonSerializer.SerializeAsync(
            stream, 
            Messages, s_serializerContext.ObservableCollectionMessageViewModel);
    }

    private async Task ExportCallbackAsync(Stream stream)
    {
        if (Messages is null)
        {
            return;
        }

        await using var writer = new StreamWriter(stream);

        for (var i = 0; i < Messages.Count; i++)
        {
            var message = Messages[i];

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
