using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace AI.Model.Json.Chat;

[DataContract]
public class ChatMessage
{
    [DataMember(Name = "role")]
    [JsonPropertyName("role")]
    public string? Role { get; set; }

    [DataMember(Name = "content")]
    [JsonPropertyName("content")]
    public string? Content { get; set; }

    [DataMember(Name = "name")]
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [DataMember(Name = "function_call")]
    [JsonPropertyName("function_call")]
    public ChatMessageFunctionCall? FunctionCall { get; set; }
}
