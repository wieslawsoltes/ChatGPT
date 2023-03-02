using System.Text.Json.Serialization;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace ChatGPT.ViewModels;

public partial class SettingsViewModel : ObservableObject
{
    private ChatSettingsViewModel? _chatSettings;
    private ActionsViewModel? _actions;
    private string? _theme;

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

    [JsonPropertyName("chatSettings")]
    public ChatSettingsViewModel? ChatSettings
    {
        get => _chatSettings;
        set => SetProperty(ref _chatSettings, value);
    }

    [JsonPropertyName("theme")]
    public string? Theme
    {
        get => _theme;
        set => SetProperty(ref _theme, value);
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
