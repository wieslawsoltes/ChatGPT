using System.Text.Json.Serialization;

namespace ChatGPT;

public class GameMessage
{
    [JsonPropertyName("story")]
    public string? Story { get; set; }

    [JsonPropertyName("option1")]
    public string? Option1 { get; set; }

    [JsonPropertyName("option2")]
    public string? Option2 { get; set; }

    [JsonPropertyName("option3")]
    public string? Option3 { get; set; }
}
