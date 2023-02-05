using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;

namespace ChatGPT.UI.ViewModels;

public partial class MainViewModel : ObservableObject
{
    [ObservableProperty] private ObservableCollection<MessageViewModel>? _messages;
    [ObservableProperty] private decimal _temperature;
    [ObservableProperty] private int _maxTokens;
}
