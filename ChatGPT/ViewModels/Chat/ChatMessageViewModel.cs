using System;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace ChatGPT.ViewModels.Chat;

public class ChatMessageViewModel : ObservableObject
{
    private string? _role;
    private string? _message;
    private string? _format;
    private bool _isSent;
    private bool _isAwaiting;
    private bool _isError;
    private bool _canRemove;
    private bool _isEditing;
    private ChatMessageViewModel? _result;
    private Func<ChatMessageViewModel, Task>? _send;
    private Func<ChatMessageViewModel, Task>? _copy;
    private Action<ChatMessageViewModel>? _remove;

    public ChatMessageViewModel()
    {
        SendCommand = new AsyncRelayCommand(async _ => await SendAction());

        EditCommand = new RelayCommand<string>(EditAction);

        CopyCommand = new RelayCommand(CopyAction);

        RemoveCommand = new RelayCommand(RemoveAction);
    }

    [JsonPropertyName("role")]
    public string? Role
    {
        get => _role;
        set => SetProperty(ref _role, value);
    }

    [JsonPropertyName("message")]
    public string? Message
    {
        get => _message;
        set => SetProperty(ref _message, value);
    }

    [JsonPropertyName("format")]
    public string? Format
    {
        get => _format;
        set => SetProperty(ref _format, value);
    }

    [JsonPropertyName("isSent")]
    public bool IsSent
    {
        get => _isSent;
        set => SetProperty(ref _isSent, value);
    }

    [JsonPropertyName("isAwaiting")]
    public bool IsAwaiting
    {
        get => _isAwaiting;
        set => SetProperty(ref _isAwaiting, value);
    }

    [JsonPropertyName("isError")]
    public bool IsError
    {
        get => _isError;
        set => SetProperty(ref _isError, value);
    }

    [JsonPropertyName("canRemove")]
    public bool CanRemove
    {
        get => _canRemove;
        set => SetProperty(ref _canRemove, value);
    }

    [JsonIgnore]
    public bool IsEditing
    {
        get => _isEditing;
        set => SetProperty(ref _isEditing, value);
    }

    [JsonPropertyName("result")]
    public ChatMessageViewModel? Result
    {
        get => _result;
        set => SetProperty(ref _result, value);
    }

    [JsonIgnore]
    public IAsyncRelayCommand SendCommand { get; }

    [JsonIgnore]
    public IRelayCommand EditCommand { get; }

    [JsonIgnore]
    public IRelayCommand CopyCommand { get; }

    [JsonIgnore]
    public IRelayCommand RemoveCommand { get; }

    private async Task SendAction()
    {
        if (_send is { })
        {
            IsEditing = false;
            await _send(this);
        }
    }

    private void EditAction(string? status)
    {
        switch (status)
        {
            case "Edit":
            {
                EditingState();
                break;
            }
            case "Cancel":
            {
                CanceledState();
                break;
            }
            case "NewLine":
            {
                NewLineState();
                break;
            }
        }
    }

    private void EditingState()
    {
        if (!IsEditing && IsSent)
        {
            IsSent = false;
            IsEditing = true;
        }
    }

    private void CanceledState()
    {
        if (IsEditing)
        {
            IsSent = true;
            IsEditing = false;
        }
    }

    private void NewLineState()
    {
        if (!IsSent)
        {
            // TODO: Use caret position to insert new line.
            Message += Environment.NewLine;
        }
    }

    private void CopyAction()
    {
        _copy?.Invoke(this);
    }

    private void RemoveAction()
    {
        _remove?.Invoke(this);
    }

    public void SetSendAction(Func<ChatMessageViewModel, Task>? send)
    {
        _send = send;
    }

    public void SetCopyAction(Func<ChatMessageViewModel, Task>? copy)
    {
        _copy = copy;
    }

    public void SetRemoveAction(Action<ChatMessageViewModel>? remove)
    {
        _remove = remove;
    }
}
