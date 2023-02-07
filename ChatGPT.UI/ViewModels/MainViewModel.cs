using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using ChatGPT.UI.Model;
using ChatGPT.UI.Services;
using CommunityToolkit.Mvvm.ComponentModel;

namespace ChatGPT.UI.ViewModels;

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

    private ObservableCollection<MessageViewModel>? _messages;
    private MessageViewModel? _currentMessage;
    private SettingsViewModel? _settings;
    private bool _isEnabled;

    public MainViewModel()
    {
        async Task NewAction()
        {
            NewCallback();
            await Task.Yield();
        }

        async Task OpenAction()
        {
            await ApplicationService.OpenFile(
                OpenCallbackAsync, 
                new List<string>(new[] { "Json", "All" }), 
                "Open");
        }

        async Task SaveAction()
        {
            await ApplicationService.SaveFile(
                SaveCallbackAsync, 
                new List<string>(new[] { "Json", "All" }), 
                "Save", 
                "messages", 
                "json");
        }

        async Task ExportAction()
        {
            await ApplicationService.SaveFile(
                ExportCallbackAsync, 
                new List<string>(new[] { "Text", "All" }), 
                "Export", 
                "messages", 
                "txt");
        }

        var actions = new ActionsViewModel
        {
            New = NewAction,
            Open = OpenAction,
            Save = SaveAction,
            Export = ExportAction,
            Exit = ApplicationService.Exit
        };

        _settings = new SettingsViewModel(actions)
        {
            Temperature = 0.7m,
            MaxTokens = 256
        };

        _messages = new ObservableCollection<MessageViewModel>();
        _isEnabled = true;

        var welcomeItem = new MessageViewModel()
        {
            Prompt = "",
            Message = "Hi! I'm Clippy, your Windows Assistant. Would you like to get some assistance?",
            IsSent = false
        };
        welcomeItem.SetSendAction(Send);
        _messages.Add(welcomeItem);
        _currentMessage = welcomeItem;
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

    private async Task Send(MessageViewModel sendMessage)
    {
        if (Messages is null)
        {
            return;
        }

        if (string.IsNullOrEmpty(sendMessage.Prompt))
        {
            return;
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
                promptMessage = new MessageViewModel();
                promptMessage.SetSendAction(Send);
                Messages.Add(promptMessage);
            }

            var prompt = sendMessage.Prompt;

            promptMessage.Message = sendMessage.Prompt;
            promptMessage.Prompt = "";
            promptMessage.IsSent = true;

            CurrentMessage = promptMessage;
            promptMessage.IsAwaiting = true;

            var temperature = Settings?.Temperature ?? 0.6m;
            var maxTokens = Settings?.MaxTokens ?? 100;
            var apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY");
            var restoreApiKey = false;

            if (!string.IsNullOrWhiteSpace(Settings?.ApiKey))
            {
                Environment.SetEnvironmentVariable("OPENAI_API_KEY", Settings.ApiKey);
                restoreApiKey = true;
            }

            var responseData = await ChatService.GetResponseDataAsync(prompt, temperature, maxTokens);
            if (resultMessage is null)
            {
                resultMessage = new MessageViewModel()
                {
                    IsSent = false
                };
                resultMessage.SetSendAction(Send);
                Messages.Add(resultMessage);
            }
            else
            {
                resultMessage.IsSent = true;
            }

            if (responseData is CompletionsResponseError error)
            {
                var message = error.Error?.Message;
                resultMessage.Message = message ?? "Unknown error.";
                resultMessage.IsError = true;
            }
            else if (responseData is CompletionsResponseSuccess success)
            {
                var message = success.Choices?.FirstOrDefault()?.Text?.Trim();
                resultMessage.Message = message ?? "";
                resultMessage.IsError = false;
            }

            resultMessage.Prompt = "";

            if (Messages.LastOrDefault() == resultMessage)
            {
                resultMessage.IsSent = false;
            }

            CurrentMessage = resultMessage;

            promptMessage.IsAwaiting = false;
            promptMessage.Result = resultMessage;

            if (restoreApiKey && !string.IsNullOrWhiteSpace(apiKey))
            {
                Environment.SetEnvironmentVariable("OPENAI_API_KEY", apiKey);
            }
        }
        catch (Exception)
        {
            // ignored
        }

        IsEnabled = true;
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

        Messages[0].IsSent = false;

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

                message.SetSendAction(Send);
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
}
