using System.Text.Json.Serialization;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace ChatGPT.UI.ViewModels;

public partial class SettingsViewModel : ObservableObject
{
    private decimal _temperature;
    private int _maxTokens;
    private string? _apiKey;

    public SettingsViewModel() : this(null)
    {
    }

    public SettingsViewModel(ActionsViewModel? actionsViewModel)
    {
        NewCommand = new AsyncRelayCommand(async () =>
        {
            if (actionsViewModel?.New is { })
            {
                await actionsViewModel.New();
            }
        });

        OpenCommand = new AsyncRelayCommand(async () =>
        {
            if (actionsViewModel?.Open is { })
            {
                await actionsViewModel.Open();
            }
        });

        SaveCommand = new AsyncRelayCommand(async () =>
        {
            if (actionsViewModel?.Save is { })
            {
                await actionsViewModel.Save();
            }
        });

        ExportCommand = new AsyncRelayCommand(async () =>
        {
            if (actionsViewModel?.Export is { })
            {
                await actionsViewModel.Export();
            }
        });

        ExitCommand = new RelayCommand(() =>
        {
            actionsViewModel?.Exit?.Invoke();
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
}
