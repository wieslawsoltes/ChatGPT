using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Interactivity;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace ChatGPT.UI;

public partial class ItemViewModel : ObservableObject
{
    [ObservableProperty] private string? _prompt;
}

public partial class WelcomeItemViewModel : ItemViewModel
{
    [ObservableProperty] private string? _message;
    [ObservableProperty] private bool _isSent;

    public WelcomeItemViewModel(Func<WelcomeItemViewModel, Task> send)
    {
        SendCommand = new AsyncRelayCommand(async _ => await send(this));
    }

    public IAsyncRelayCommand SendCommand { get; }
}

public partial class ChoiceItemViewModel : ItemViewModel
{
    [ObservableProperty] private string? _message;
}

public partial class MainViewModel : ObservableObject
{
    [ObservableProperty] private ObservableCollection<ItemViewModel>? _items;
    [ObservableProperty] private decimal _temperature;
    [ObservableProperty] private int _maxTokens;
}

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
            Items = new ObservableCollection<ItemViewModel>()
        };

        var welcomeItem = new WelcomeItemViewModel(SubmitOnClick)
        {
            Prompt = "",
            Message = "Hi! I'm Clippy, your Windows Assistant. Would you like to get some assistance?",
            IsSent = false
        };
        _mainViewModel.Items.Add(welcomeItem);

        DataContext = _mainViewModel;
    }

    private async Task SubmitOnClick(WelcomeItemViewModel welcomeItem)
    {
        if (_mainViewModel.Items is null)
        {
            return;
        }

        if (string.IsNullOrEmpty(welcomeItem.Prompt))
        {
            return;
        }

        IsEnabled = false;

        welcomeItem.IsSent = true;

        var promptItem = new WelcomeItemViewModel(SubmitOnClick)
        {
            Prompt = "",
            Message = welcomeItem.Prompt,
            IsSent = true
        };
        _mainViewModel.Items.Add(promptItem);

        var prompt = welcomeItem.Prompt;
        var temperature = _mainViewModel.Temperature;
        var maxTokens  = _mainViewModel.MaxTokens;
        var responseData = await ChatService.GetResponseDataAsync(prompt, temperature, maxTokens);

        var responseItem = new WelcomeItemViewModel(SubmitOnClick)
        {
            Prompt = "",
            Message = responseData.Choices?.FirstOrDefault()?.Text.Trim(),
            IsSent = false
        };
        _mainViewModel.Items.Add(responseItem);

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
