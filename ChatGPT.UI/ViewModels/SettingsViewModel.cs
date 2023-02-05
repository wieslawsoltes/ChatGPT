using System;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace ChatGPT.UI.ViewModels;

public partial class SettingsViewModel : ObservableObject
{
    [ObservableProperty] private decimal _temperature;
    [ObservableProperty] private int _maxTokens;
    
    public SettingsViewModel(Action exit)
    {
        ExitCommand = new RelayCommand(exit);
    }

    public IRelayCommand ExitCommand { get; }
}
