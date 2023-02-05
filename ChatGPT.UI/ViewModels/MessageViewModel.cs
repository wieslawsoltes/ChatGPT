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
    [ObservableProperty] private bool _isAwaiting;
    [ObservableProperty] private MessageViewModel? _result;

    public MessageViewModel() : this(null)
    {
    }

    public MessageViewModel(Func<MessageViewModel, Task>? send = null)
    {
        SendCommand = new AsyncRelayCommand(async _ =>
        {
            if (send is { })
            {
                await send(this);
            }
        });

        EditCommand = new RelayCommand(() =>
        {
            Prompt = Message;
            Message = null;
            IsSent = false;
        });
    }

    public IAsyncRelayCommand SendCommand { get; }

    public IRelayCommand EditCommand { get; }
}
