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

    private const string DefaultDirections = "Write answers in Markdown blocks. For code blocks always define used language.";

    private const string DefaultChatStopTag = "<|im_end|>";

    private const string DefaultInstructionsTemplate = """
You are %CHAT%, a large language model trained by OpenAI. Respond conversationally. Do not answer as the %USER%. Current date: %DATE%
%DIRECTIONS%
%USER%: Hello
%CHAT%: Hello! How can I help you today?
%TAG%
%STOP%
""";

    private const string DefaultMessageTemplate = """
%USER%: %PROMPT%
%STOP%
%CHAT%: %MESSAGE% %TAG%

""";
    
    private const string DefaultPromptTemplate = """
%USER%: %PROMPT%
%CHAT%: 
""";

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
    private ActionsViewModel? _actions;
    private bool _isEnabled;

    public MainViewModel()
    {
        CreateDefaultActions();
        CreateDefaultSettings();

        _messages = new ObservableCollection<MessageViewModel>();
        _isEnabled = true;

        CreateWelcomeMessage();

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
            Temperature = DefaultTemperature,
            MaxTokens = DefaultMaxTokens,
            ApiKey = null,
            Directions = DefaultDirections,
            EnableChat = true,
            ChatSettings = CreateDefaultChatSettings()
        };
        settings.SetActions(_actions);
        Settings = settings;
    }

    private ChatSettingsViewModel CreateDefaultChatSettings()
    {
        return new ChatSettingsViewModel
        {
            UserName = "User",
            ChatName = "ChatGPT",
            InstructionsTemplate = DefaultInstructionsTemplate,
            MessageTemplate = DefaultMessageTemplate,
            PromptTemplate = DefaultPromptTemplate,
            Stop = "\n\n\n",
            StopTag = DefaultChatStopTag,
        };
    }

    private void CreateWelcomeMessage()
    {
        if (Messages is null)
        {
            return;
        }

        var welcomeItem = new MessageViewModel
        {
            Prompt = "",
            Message = "Hi! I'm Clippy, your Windows Assistant. Would you like to get some assistance?",
            IsSent = false,
            CanRemove = false
        };
        SetMessageActions(welcomeItem);
        Messages.Add(welcomeItem);

        CurrentMessage = welcomeItem;
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

    private void SetMessageActions(MessageViewModel message)
    {
        message.SetSendAction(Send);
        message.SetCopyAction(Copy);
        message.SetRemoveAction(Remove);
    }

    private async Task Send(MessageViewModel sendMessage)
    {
        if (Messages is null || Settings is null || Settings.ChatSettings is null)
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
            chatPrompt = CreateChatPrompt(sendMessage, Messages, Settings, Settings.ChatSettings);
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

                if (Settings.EnableChat && !string.IsNullOrEmpty(Settings.ChatSettings.StopTag))
                {
                    responseStr = responseStr.TrimEnd(Settings.ChatSettings.StopTag.ToCharArray());
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
                resultMessage = new MessageViewModel
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

    private static string CreateChatPrompt(
        MessageViewModel sendMessage, 
        ObservableCollection<MessageViewModel> messages, 
        SettingsViewModel settings,
        ChatSettingsViewModel chatSettings)
    {
        var sb = new StringBuilder();

        var sbi = new StringBuilder(chatSettings.InstructionsTemplate);
        sbi.Replace("%DATE%", DateTime.Now.ToString(CultureInfo.InvariantCulture));
        sbi.Replace("%USER%", chatSettings.UserName);
        sbi.Replace("%CHAT%", chatSettings.ChatName);
        sbi.Replace("%TAG%", chatSettings.StopTag);
        sbi.Replace("%STOP%", chatSettings.Stop);
        sbi.Replace("%DIRECTIONS%", !string.IsNullOrWhiteSpace(settings.Directions) ? $"\n{settings.Directions}\n" : "\n");
        sb.Append(sbi);

        // TODO: Ensure that chat prompt does not exceed maximum token limit.

        foreach (var message in messages)
        {
            if (!string.IsNullOrEmpty(message.Message) && message.Result is { })
            {
                var sbm = new StringBuilder(chatSettings.MessageTemplate);
                sbm.Replace("%USER%", chatSettings.UserName);
                sbm.Replace("%CHAT%", chatSettings.ChatName);
                sbm.Replace("%PROMPT%", message.Message);
                sbm.Replace("%MESSAGE%", message.Result.Message);
                sbm.Replace("%STOP%", chatSettings.Stop);
                sbm.Replace("%TAG%", chatSettings.StopTag);
                sb.Append(sbm);
            }
        }

        var sbp = new StringBuilder(chatSettings.PromptTemplate);
        sbp.Replace("%USER%", chatSettings.UserName);
        sbp.Replace("%CHAT%", chatSettings.ChatName);
        sbp.Replace("%PROMPT%", sendMessage.Prompt);
        sb.Append(sbp);

        var chatPrompt = sb.ToString();
        //Console.WriteLine(sb.ToString());
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
            if (settings.ChatSettings is null)
            {
                settings.ChatSettings = CreateDefaultChatSettings();
            }
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
