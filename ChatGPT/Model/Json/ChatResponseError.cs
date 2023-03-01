using System.Text.Json.Serialization;

namespace ChatGPT.Model.Json;

public class ChatResponseError : ChatResponse
{
    [JsonPropertyName("error")]
    public ChatError? Error { get; set; }
}
