using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ChatGPT.UI.Model.Json;

public class CompletionsRequestBody
{
    [JsonPropertyName("model")]
    public string? Model { get; set; }

    [JsonPropertyName("prompt")]
    public string? Prompt { get; set; }

    [JsonPropertyName("suffix")]
    public string? Suffix { get; set; }

    [JsonPropertyName("max_tokens")]
    public int MaxTokens { get; set; } = 16;

    [JsonPropertyName("temperature")]
    public decimal Temperature { get; set; } = 1;

    [JsonPropertyName("top_p")]
    public decimal TopP { get; set; } = 1;

    [JsonPropertyName("n")]
    public int N { get; set; } = 1;

    [JsonPropertyName("stream")]
    public bool Stream { get; set; }

    [JsonPropertyName("logprobs")]
    public int? Logprobs { get; set; }

    [JsonPropertyName("echo")]
    public bool Echo { get; set; }

    [JsonPropertyName("stop")]
    public string? Stop { get; set; }

    [JsonPropertyName("presence_penalty")]
    public decimal PresencePenalty { get; set; }

    [JsonPropertyName("frequency_penalty")]
    public decimal FrequencyPenalty { get; set; }

    [JsonPropertyName("best_of")]
    public int BestOf { get; set; } = 1;

    [JsonPropertyName("logit_bias")]
    public Dictionary<string, decimal>? LogitBias { get; set; }

    [JsonPropertyName("user")]
    public string? User { get; set; }
}
