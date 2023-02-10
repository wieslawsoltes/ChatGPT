using System.Text.Json.Serialization;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace ChatGPT.ViewModels;

public partial class SettingsViewModel : ObservableObject
{
    private decimal _temperature;
    private int _maxTokens;
    private string? _apiKey;
    private string? _model;
    private string? _directions;
    private bool _enableChat;
    private ChatSettingsViewModel? _chatSettings;
    private ActionsViewModel? _actions;

    public SettingsViewModel()
    {
        NewCommand = new AsyncRelayCommand(async () =>
        {
            if (_actions?.New is { })
            {
                await _actions.New();
            }
        });

        OpenCommand = new AsyncRelayCommand(async () =>
        {
            if (_actions?.Open is { })
            {
                await _actions.Open();
            }
        });

        SaveCommand = new AsyncRelayCommand(async () =>
        {
            if (_actions?.Save is { })
            {
                await _actions.Save();
            }
        });

        ExportCommand = new AsyncRelayCommand(async () =>
        {
            if (_actions?.Export is { })
            {
                await _actions.Export();
            }
        });

        ExitCommand = new RelayCommand(() =>
        {
            _actions?.Exit?.Invoke();
        });
    }

    [JsonPropertyName("temperature")]
    public decimal Temperature
    {
        get => _temperature;
        set => SetProperty(ref _temperature, value);
    }

    [JsonPropertyName("maxTokens")]
    public int MaxTokens
    {
        get => _maxTokens;
        set => SetProperty(ref _maxTokens, value);
    }

    [JsonPropertyName("apiKey")]
    public string? ApiKey
    {
        get => _apiKey;
        set => SetProperty(ref _apiKey, value);
    }

    [JsonPropertyName("model")]
    public string? Model
    {
        get => _model;
        set => SetProperty(ref _model, value);
    }

    [JsonPropertyName("directions")]
    public string? Directions
    {
        get => _directions;
        set => SetProperty(ref _directions, value);
    }

    [JsonPropertyName("enableChat")]
    public bool EnableChat
    {
        get => _enableChat;
        set => SetProperty(ref _enableChat, value);
    }

    [JsonPropertyName("chatSettings")]
    public ChatSettingsViewModel? ChatSettings
    {
        get => _chatSettings;
        set => SetProperty(ref _chatSettings, value);
    }

    [JsonIgnore]
    public IAsyncRelayCommand NewCommand { get; }

    [JsonIgnore]
    public IAsyncRelayCommand OpenCommand { get; }

    [JsonIgnore]
    public IAsyncRelayCommand SaveCommand { get; }

    [JsonIgnore]
    public IAsyncRelayCommand ExportCommand { get; }

    [JsonIgnore]
    public IRelayCommand ExitCommand { get; }

    public void SetActions(ActionsViewModel? actions)
    {
        _actions = actions;
    }
}
