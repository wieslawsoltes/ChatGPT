using System;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace ChatGPT.UI.ViewModels;

public partial class SettingsViewModel : ObservableObject
{
    private decimal _temperature;
    private int _maxTokens;
    private string? _apiKey;

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

    public decimal Temperature
    {
        get => _temperature;
        set => SetProperty(ref _temperature, value);
    }

    public int MaxTokens
    {
        get => _maxTokens;
        set => SetProperty(ref _maxTokens, value);
    }

    public string? ApiKey
    {
        get => _apiKey;
        set => SetProperty(ref _apiKey, value);
    }

    public IRelayCommand ExitCommand { get; }
}
