using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace AI.Model.Json.Chat;

[DataContract]
public class ChatRequestBody
{
    [DataMember(Name = "model")]
    [JsonPropertyName("model")]
    public string? Model { get; set; }

    [DataMember(Name = "messages")]
    [JsonPropertyName("messages")]
    public ChatMessage[]? Messages { get; set; }

    [DataMember(Name = "temperature")]
    [JsonPropertyName("temperature")]
    public decimal Temperature { get; set; } = 1;

    [DataMember(Name = "top_p")]
    [JsonPropertyName("top_p")]
    public decimal TopP { get; set; } = 1;

    [DataMember(Name = "n")]
    [JsonPropertyName("n")]
    public int N { get; set; } = 1;

    [DataMember(Name = "stream")]
    [JsonPropertyName("stream")]
    public bool Stream { get; set; }

    [DataMember(Name = "stop")]
    [JsonPropertyName("stop")]
    public string? Stop { get; set; }

    [DataMember(Name = "max_tokens")]
    [JsonPropertyName("max_tokens")]
    public int MaxTokens { get; set; } = 16;

    [DataMember(Name = "presence_penalty")]
    [JsonPropertyName("presence_penalty")]
    public decimal PresencePenalty { get; set; }

    [DataMember(Name = "frequency_penalty")]
    [JsonPropertyName("frequency_penalty")]
    public decimal FrequencyPenalty { get; set; }

    [DataMember(Name = "logit_bias")]
    [JsonPropertyName("logit_bias")]
    public Dictionary<string, decimal>? LogitBias { get; set; }

    [DataMember(Name = "user")]
    [JsonPropertyName("user")]
    public string? User { get; set; }
}
