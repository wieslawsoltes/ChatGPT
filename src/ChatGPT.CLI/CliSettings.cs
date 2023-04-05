namespace ChatGPT.CLI;

internal class CliSettings
{
    // Input and output
    public FileInfo[]? InputFiles { get; set; } = null;
    public DirectoryInfo? InputDirectory { get; set; } = null;
    public FileInfo[]? OutputFiles { get; set; } = null;
    public DirectoryInfo? OutputDirectory { get; set; } = null;
    public string[] Pattern { get; set; } = {"*.*"};
    public bool Recursive { get; set; } = true;
    public string Extension { get; set; } = "txt";
    // Chat settings
    public FileInfo? SettingsFile { get; set; } = null;
    public decimal Temperature { get; set; } = 0.7m;
    public decimal TopP { get; set; } = 1m;
    public decimal PresencePenalty { get; set; } = 0m;
    public decimal FrequencyPenalty { get; set; } = 0m;
    public int MaxTokens { get; set; } = 2000;
    public string? ApiKey { get; set; } = null;
    public string? Model { get; set; } = "gpt-3.5-turbo";
    public string? Directions { get; set; } = "You are a helpful assistant.";
    // Job settings
    public int Threads { get; set; } = 1;
    public bool Quiet { get; set; } = false;
}
