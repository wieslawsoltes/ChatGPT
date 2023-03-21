using System.Text.Json.Serialization;
using CommunityToolkit.Mvvm.ComponentModel;

namespace ChatGPT.ViewModels.Chat;

public class ChatResultViewModel : ObservableObject
{
    private string? _message;
    private bool _isError;

    [JsonPropertyName("name")]
    public string? Message
    {
        get => _message;
        set => SetProperty(ref _message, value);
    }

    [JsonPropertyName("isError")]
    public bool IsError
    {
        get => _isError;
        set => SetProperty(ref _isError, value);
    }
}
