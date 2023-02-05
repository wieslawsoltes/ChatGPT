using System;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace ChatGPT.UI.ViewModels;

public partial class MessageViewModel : ObservableObject
{
    [ObservableProperty] private string? _prompt;
    [ObservableProperty] private string? _message;
    [ObservableProperty] private bool _isSent;

    public MessageViewModel(Func<MessageViewModel, Task> send)
    {
        SendCommand = new AsyncRelayCommand(async _ => await send(this));
    }

    public IAsyncRelayCommand SendCommand { get; }
}
