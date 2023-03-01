namespace ChatGPT;

internal static class Constants
{
    public const string EnvironmentVariableApiKey = "OPENAI_API_KEY";

    public const decimal DefaultTemperature = 0.7m;

    public const int DefaultMaxTokens = 256;

    public const string DefaultDirections = "You are a helpful assistant. Write answers in Markdown blocks. For code blocks always define used language.";

    public const string DefaultMessageFormat = "Markdown";

    public const string TextMessageFormat = "Text";
    
    public const string MarkdownMessageFormat = "Markdown";
    
    public const string HtmlMessageTextFormat = "Html";
}
