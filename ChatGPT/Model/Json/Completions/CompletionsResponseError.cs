using System.Text.Json.Serialization;

namespace ChatGPT.Model.Json;

public class CompletionsResponseError : CompletionsResponse
{
    [JsonPropertyName("error")]
    public CompletionsError? Error { get; set; }
}
