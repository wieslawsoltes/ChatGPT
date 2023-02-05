using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Interactivity;
using ChatGPT.UI.ViewModels;

namespace ChatGPT.UI;

public partial class MainWindow : Window
{
    private readonly MainViewModel _mainViewModel;

    public MainWindow()
    {
        InitializeComponent();

        _mainViewModel = new MainViewModel
        {
            Temperature = 0.6m,
            MaxTokens = 100,
            Messages = new ObservableCollection<MessageViewModel>()
        };

        var welcomeItem = new MessageViewModel(SubmitOnClick)
        {
            Prompt = "",
            Message = "Hi! I'm Clippy, your Windows Assistant. Would you like to get some assistance?",
            IsSent = false
        };
        _mainViewModel.Messages.Add(welcomeItem);

        DataContext = _mainViewModel;
    }

    private async Task SubmitOnClick(MessageViewModel message)
    {
        if (_mainViewModel.Messages is null)
        {
            return;
        }

        if (string.IsNullOrEmpty(message.Prompt))
        {
            return;
        }

        IsEnabled = false;

        message.IsSent = true;

        var promptItem = new MessageViewModel(SubmitOnClick)
        {
            Prompt = "",
            Message = message.Prompt,
            IsSent = true
        };
        _mainViewModel.Messages.Add(promptItem);

        var prompt = message.Prompt;
        var temperature = _mainViewModel.Temperature;
        var maxTokens  = _mainViewModel.MaxTokens;
        var responseData = await ChatService.GetResponseDataAsync(prompt, temperature, maxTokens);

        var responseItem = new MessageViewModel(SubmitOnClick)
        {
            Prompt = "",
            Message = responseData.Choices?.FirstOrDefault()?.Text.Trim(),
            IsSent = false
        };
        _mainViewModel.Messages.Add(responseItem);

        IsEnabled = true;
    }

    private void Exit_OnClick(object? sender, RoutedEventArgs e)
    {
        if (Application.Current?.ApplicationLifetime is IControlledApplicationLifetime lifetime)
        {
            lifetime.Shutdown();
        }
    }
}
