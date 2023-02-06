using System.Text.Json.Serialization;

namespace ChatGPT.UI.Model;

public class CompletionsResponseError : CompletionsResponse
{
    [JsonPropertyName("error")]
    public CompletionsError? Error { get; set; }
}
