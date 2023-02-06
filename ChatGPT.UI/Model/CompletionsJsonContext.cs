using System.Text.Json.Serialization;

namespace ChatGPT.UI.Model;

[JsonSerializable(typeof(CompletionsChoice))]
[JsonSerializable(typeof(CompletionsRequestBody))]
[JsonSerializable(typeof(CompletionsResponse))]
[JsonSerializable(typeof(CompletionsUsage))]
public partial class CompletionsJsonContext : JsonSerializerContext
{
}
