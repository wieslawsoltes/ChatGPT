using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using ChatGPT.UI.Model;
using ChatGPT.UI.Services;
using CommunityToolkit.Mvvm.ComponentModel;

namespace ChatGPT.UI.ViewModels;

public partial class MainViewModel : ObservableObject
{
    [ObservableProperty] private ObservableCollection<MessageViewModel>? _messages;
    [ObservableProperty] private MessageViewModel? _currentMessage;
    [ObservableProperty] private SettingsViewModel? _settings;
    [ObservableProperty] private bool _isEnabled;

    public MainViewModel() : this(null)
    {
    }

    public MainViewModel(Action? exit = null)
    {
        _settings = new SettingsViewModel(exit)
        {
            Temperature = 0.6m,
            MaxTokens = 100
        };

        _messages = new ObservableCollection<MessageViewModel>();
        _isEnabled = true;

        var welcomeItem = new MessageViewModel(Send)
        {
            Prompt = "",
            Message = "Hi! I'm Clippy, your Windows Assistant. Would you like to get some assistance?",
            IsSent = false
        };
        _messages.Add(welcomeItem);
        _currentMessage = welcomeItem;
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
                promptMessage = new MessageViewModel(Send);
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
                resultMessage = new MessageViewModel(Send)
                {
                    IsSent = false
                };
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
}
