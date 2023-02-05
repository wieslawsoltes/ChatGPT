using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;

namespace ChatGPT.UI.ViewModels;

public partial class MainViewModel : ObservableObject
{
    [ObservableProperty] private ObservableCollection<MessageViewModel>? _messages;
    [ObservableProperty] private MessageViewModel? _currentMessage;
    [ObservableProperty] private SettingsViewModel? _settings;
    [ObservableProperty] private bool _isEnabled;

    public MainViewModel(Action exit)
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
    
    private async Task Send(MessageViewModel message)
    {
        if (Messages is null)
        {
            return;
        }

        if (string.IsNullOrEmpty(message.Prompt))
        {
            return;
        }

        IsEnabled = false;

        message.IsSent = true;

        var promptItem = new MessageViewModel(Send)
        {
            Prompt = "",
            Message = message.Prompt,
            IsSent = true
        };
        Messages.Add(promptItem);
        CurrentMessage = promptItem;

        promptItem.IsAwaiting = true;

        var prompt = message.Prompt;
        var temperature = Settings?.Temperature ?? 0.6m;
        var maxTokens = Settings?.MaxTokens ?? 100;
        var responseData = await ChatService.GetResponseDataAsync(prompt, temperature, maxTokens);
        var responseItem = new MessageViewModel(Send)
        {
            Prompt = "",
            Message = responseData.Choices?.FirstOrDefault()?.Text.Trim(),
            IsSent = false
        };
        Messages.Add(responseItem);
        CurrentMessage = responseItem;

        promptItem.IsAwaiting = false;
        promptItem.Result = responseItem;

        IsEnabled = true;
    }
}
