using System.Collections.ObjectModel;
using System.Text.Json.Serialization;
using CommunityToolkit.Mvvm.ComponentModel;

namespace ChatGPT.ViewModels;

public class ChatViewModel : ObservableObject
{
    private string? _name;
    private ObservableCollection<MessageViewModel> _messages;
    private MessageViewModel? _currentMessage;

    public ChatViewModel()
    {
        _messages = new ObservableCollection<MessageViewModel>();
    }

    [JsonPropertyName("name")]
    public string? Name
    {
        get => _name;
        set => SetProperty(ref _name, value);
    }

    [JsonPropertyName("messages")]
    public ObservableCollection<MessageViewModel> Messages
    {
        get => _messages;
        set => SetProperty(ref _messages, value);
    }

    [JsonPropertyName("currentMessage")]
    public MessageViewModel? CurrentMessage
    {
        get => _currentMessage;
        set => SetProperty(ref _currentMessage, value);
    }
}
