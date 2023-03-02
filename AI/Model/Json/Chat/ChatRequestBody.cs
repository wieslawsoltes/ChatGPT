using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace AI.Model.Json.Chat;

public class ChatRequestBody
{
    [JsonPropertyName("model")]
    public string? Model { get; set; }

    [JsonPropertyName("messages")]
    public ChatMessage[]? Messages { get; set; }

    [JsonPropertyName("temperature")]
    public decimal Temperature { get; set; } = 1;

    [JsonPropertyName("top_p")]
    public decimal TopP { get; set; } = 1;

    [JsonPropertyName("n")]
    public int N { get; set; } = 1;

    [JsonPropertyName("stream")]
    public bool Stream { get; set; }

    [JsonPropertyName("stop")]
    public string? Stop { get; set; }

    [JsonPropertyName("max_tokens")]
    public int MaxTokens { get; set; } = 16;

    [JsonPropertyName("presence_penalty")]
    public decimal PresencePenalty { get; set; }

    [JsonPropertyName("frequency_penalty")]
    public decimal FrequencyPenalty { get; set; }

    [JsonPropertyName("logit_bias")]
    public Dictionary<string, decimal>? LogitBias { get; set; }

    [JsonPropertyName("user")]
    public string? User { get; set; }
}
