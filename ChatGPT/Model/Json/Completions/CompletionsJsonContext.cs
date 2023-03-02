using System.Text.Json.Serialization;

namespace ChatGPT.Model.Json;

[JsonSerializable(typeof(CompletionsRequestBody))]
[JsonSerializable(typeof(CompletionsResponseSuccess))]
[JsonSerializable(typeof(CompletionsChoice))]
[JsonSerializable(typeof(CompletionsUsage))]
[JsonSerializable(typeof(CompletionsResponseError))]
[JsonSerializable(typeof(CompletionsError))]
public partial class CompletionsJsonContext : JsonSerializerContext
{
}
