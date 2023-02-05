using System;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace ChatGPT.UI.ViewModels;

public partial class SettingsViewModel : ObservableObject
{
    [ObservableProperty] private decimal _temperature;
    [ObservableProperty] private int _maxTokens;
    [ObservableProperty] private string? _apiKey;

    public SettingsViewModel() : this(null)
    {
    }
    
    public SettingsViewModel(Action? exit = null)
    {
        ExitCommand = new RelayCommand(() =>
        {
            exit?.Invoke();
        });
    }

    public IRelayCommand ExitCommand { get; }
}
