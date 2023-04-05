using System.Text.Json.Serialization;
using CommunityToolkit.Mvvm.ComponentModel;

namespace ChatGPT.ViewModels.Chat;

public partial class ChatSettingsViewModel : ObservableObject
{
    private decimal _temperature;
    private decimal _topP;
    private decimal _presencePenalty;
    private decimal _frequencyPenalty;
    private int _maxTokens;
    private string? _apiKey;
    private string? _model;
    private string? _directions;
    private string? _format;

    [JsonConstructor]
    public ChatSettingsViewModel()
    {
        _temperature = 0.7m;
        _topP = 1m;
        _presencePenalty = 0m;
        _frequencyPenalty = 0m;
        _maxTokens = 2000;
        _apiKey = null;
        _model = "gpt-3.5-turbo";
        _directions = "You are a helpful assistant.";
    }

    [JsonPropertyName("temperature")]
    public decimal Temperature
    {
        get => _temperature;
        set => SetProperty(ref _temperature, value);
    }

    [JsonPropertyName("top_p")]
    public decimal TopP
    {
        get => _topP;
        set => SetProperty(ref _topP, value);
    }

    [JsonPropertyName("presence_penalty")]
    public decimal PresencePenalty
    {
        get => _presencePenalty;
        set => SetProperty(ref _presencePenalty, value);
    }

    [JsonPropertyName("frequency_penalty")]
    public decimal FrequencyPenalty
    {
        get => _frequencyPenalty;
        set => SetProperty(ref _frequencyPenalty, value);
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

    [JsonPropertyName("format")]
    public string? Format
    {
        get => _format;
        set => SetProperty(ref _format, value);
    }

    public ChatSettingsViewModel Copy()
    {
        return new ChatSettingsViewModel
        {
            Temperature = _temperature,
            TopP = _topP,
            PresencePenalty = _presencePenalty,
            FrequencyPenalty = _frequencyPenalty,
            MaxTokens = _maxTokens,
            ApiKey = _apiKey,
            Model = _model,
            Directions = _directions,
            Format = _format,
        };
    }
}
