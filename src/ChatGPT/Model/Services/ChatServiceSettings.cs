using AI.Model.Json.Chat;

namespace AI.Model.Services;

public class ChatServiceSettings
{
    public string? Model { get; set; }
    public ChatMessage[]? Messages { get; set; }
    public string? Suffix { get; set; }
    public decimal Temperature { get; set; }
    public int MaxTokens { get; set; }
    public decimal TopP { get; set; }
    public decimal PresencePenalty { get; set; }
    public decimal FrequencyPenalty { get; set; }
    public string? Stop { get; set; }
}
