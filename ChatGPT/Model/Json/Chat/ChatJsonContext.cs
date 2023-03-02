using System.Text.Json.Serialization;

namespace ChatGPT.Model.Json;

[JsonSerializable(typeof(ChatRequestBody))]
[JsonSerializable(typeof(ChatResponseSuccess))]
[JsonSerializable(typeof(ChatChoice))]
[JsonSerializable(typeof(ChatMessage))]
[JsonSerializable(typeof(ChatUsage))]
[JsonSerializable(typeof(ChatResponseError))]
[JsonSerializable(typeof(ChatError))]
public partial class ChatJsonContext : JsonSerializerContext
{
}
