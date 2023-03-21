using System.Text.Json.Serialization;

namespace AI.Model.Json.Chat;

public class ChatResponseError : ChatResponse
{
    [JsonPropertyName("error")]
    public ChatError? Error { get; set; }
}
