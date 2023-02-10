namespace ChatGPT;

internal static class Constants
{
    public const string EnvironmentVariableApiKey = "OPENAI_API_KEY";

    public const decimal DefaultTemperature = 0.7m;

    public const int DefaultMaxTokens = 256;

    public const string DefaultDirections = "Write answers in Markdown blocks. For code blocks always define used language.";

    public const string DefaultChatStopTag = "<|im_end|>";

    public const string DefaultInstructionsTemplate = """
You are %CHAT%, a large language model trained by OpenAI. Respond conversationally. Do not answer as the %USER%. Current date: %DATE%
%DIRECTIONS%
%USER%: Hello
%CHAT%: Hello! How can I help you today?
%TAG%
%STOP%
""";

    public const string DefaultMessageTemplate = """
%USER%: %PROMPT%
%STOP%
%CHAT%: %MESSAGE% %TAG%

""";
    
    public const string DefaultPromptTemplate = """
%USER%: %PROMPT%
%CHAT%: 
""";

}
