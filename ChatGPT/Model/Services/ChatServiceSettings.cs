namespace ChatGPT.Model.Services;

public class ChatServiceSettings
{
    public string? Prompt { get; set; }
    public decimal Temperature { get; set; }
    public int MaxTokens { get; set; }
    public string? Stop { get; set; }
}
