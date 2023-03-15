using System.Collections.ObjectModel;
using System.Text.Json.Serialization;
using ChatGPT.ViewModels.Chat;
using ChatGPT.ViewModels.Layouts;
using CommunityToolkit.Mvvm.ComponentModel;

namespace ChatGPT.ViewModels.Settings;

public class WorkspaceViewModel : ObservableObject
{
    private ObservableCollection<ChatViewModel>? _chats;
    private ChatViewModel? _currentChat;
    private ObservableCollection<PromptViewModel>? _prompts;
    private PromptViewModel? _currentPrompt;
    private ObservableCollection<LayoutViewModel>? _layouts;
    private LayoutViewModel? _currentLayout;
    private string? _theme;
    private string? _layout;
    private double _width;
    private double _height;

    [JsonPropertyName("chats")]
    public ObservableCollection<ChatViewModel>? Chats
    {
        get => _chats;
        set => SetProperty(ref _chats, value);
    }

    [JsonPropertyName("currentChat")]
    public ChatViewModel? CurrentChat
    {
        get => _currentChat;
        set => SetProperty(ref _currentChat, value);
    }
    
    [JsonPropertyName("prompts")]
    public ObservableCollection<PromptViewModel>? Prompts
    {
        get => _prompts;
        set => SetProperty(ref _prompts, value);
    }

    [JsonPropertyName("currentPrompt")]
    public PromptViewModel? CurrentPrompt
    {
        get => _currentPrompt;
        set => SetProperty(ref _currentPrompt, value);
    }

    [JsonPropertyName("layouts")]
    public ObservableCollection<LayoutViewModel>? Layouts
    {
        get => _layouts;
        set => SetProperty(ref _layouts, value);
    }

    [JsonPropertyName("currentLayout")]
    public LayoutViewModel? CurrentLayout
    {
        get => _currentLayout;
        set => SetProperty(ref _currentLayout, value);
    }

    [JsonPropertyName("theme")]
    public string? Theme
    {
        get => _theme;
        set => SetProperty(ref _theme, value);
    }

    [JsonPropertyName("layout")]
    public string? Layout
    {
        get => _layout;
        set => SetProperty(ref _layout, value);
    }

    [JsonPropertyName("width")]
    public double Width
    {
        get => _width;
        set => SetProperty(ref _width, value);
    }

    [JsonPropertyName("height")]
    public double Height
    {
        get => _height;
        set => SetProperty(ref _height, value);
    }
}
