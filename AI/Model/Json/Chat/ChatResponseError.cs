using System.Text.Json.Serialization;

namespace AI.Model.Json;

public class ChatResponseError : ChatResponse
{
    [JsonPropertyName("error")]
    public ChatError? Error { get; set; }
}
