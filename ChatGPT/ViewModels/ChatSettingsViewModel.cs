using System.Text.Json.Serialization;
using CommunityToolkit.Mvvm.ComponentModel;

namespace ChatGPT.ViewModels;

public class ChatSettingsViewModel : ObservableObject
{
    private string? _userName;
    private string? _chatName;
    private string? _instructionsTemplate;
    private string? _messageTemplate;
    private string? _promptTemplate;
    private string? _stop;
    private string? _stopTag;

    [JsonPropertyName("userName")]
    public string? UserName
    {
        get => _userName;
        set => SetProperty(ref _userName, value);
    }

    [JsonPropertyName("chatName")]
    public string? ChatName
    {
        get => _chatName;
        set => SetProperty(ref _chatName, value);
    }

    [JsonPropertyName("instructionsTemplate")]
    public string? InstructionsTemplate
    {
        get => _instructionsTemplate;
        set => SetProperty(ref _instructionsTemplate, value);
    }

    [JsonPropertyName("messageTemplate")]
    public string? MessageTemplate
    {
        get => _messageTemplate;
        set => SetProperty(ref _messageTemplate, value);
    }

    [JsonPropertyName("promptTemplate")]
    public string? PromptTemplate
    {
        get => _promptTemplate;
        set => SetProperty(ref _promptTemplate, value);
    }

    [JsonPropertyName("stop")]
    public string? Stop
    {
        get => _stop;
        set => SetProperty(ref _stop, value);
    }

    [JsonPropertyName("stopTag")]
    public string? StopTag
    {
        get => _stopTag;
        set => SetProperty(ref _stopTag, value);
    }
}
