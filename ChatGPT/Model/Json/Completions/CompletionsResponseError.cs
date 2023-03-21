using System.Text.Json.Serialization;

namespace AI.Model.Json.Completions;

public class CompletionsResponseError : CompletionsResponse
{
    [JsonPropertyName("error")]
    public CompletionsError? Error { get; set; }
}
