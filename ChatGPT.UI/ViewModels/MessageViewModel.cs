using System;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace ChatGPT.UI.ViewModels;

public class MessageViewModel : ObservableObject
{
    private string? _prompt;
    private string? _message;
    private bool _isSent;
    private bool _isAwaiting;
    private bool _isError;
    private MessageViewModel? _result;

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

        EditCommand = new RelayCommand<string>(status =>
        {
            switch (status)
            {
                case "Edit":
                {
                    Prompt = Message;
                    Message = null;
                    IsSent = false;
                    break;
                }
                case "Cancel":
                {
                    Message = Prompt;
                    Prompt = null;
                    IsSent = true;
                    break;
                }
            }
        });
    }

    public string? Prompt
    {
        get => _prompt;
        set => SetProperty(ref _prompt, value);
    }

    public string? Message
    {
        get => _message;
        set => SetProperty(ref _message, value);
    }

    public bool IsSent
    {
        get => _isSent;
        set => SetProperty(ref _isSent, value);
    }

    public bool IsAwaiting
    {
        get => _isAwaiting;
        set => SetProperty(ref _isAwaiting, value);
    }

    public bool IsError
    {
        get => _isError;
        set => SetProperty(ref _isError, value);
    }
    
    public MessageViewModel? Result
    {
        get => _result;
        set => SetProperty(ref _result, value);
    }

    public IAsyncRelayCommand SendCommand { get; }

    public IRelayCommand EditCommand { get; }
}
