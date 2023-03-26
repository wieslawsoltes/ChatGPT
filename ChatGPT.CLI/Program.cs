using ChatGPT;
using ChatGPT.ViewModels.Chat;

Defaults.ConfigureDefaultServices();

var directions = 
"""
You are a helpful assistant named Clippy.
Write answers in plain text, do not use markdown.
""";

if (args.Length == 1)
{
    directions = args[0];
}

var chat = new ChatViewModel
{
    Settings = new ChatSettingsViewModel
    {
        Temperature = 0.7m,
        TopP = 1m,
        MaxTokens = 2000,
        Model = "gpt-3.5-turbo",
        Directions = directions
    }
};

chat.AddSystemMessage(directions);

while (true)
{
    Console.Write("> ");

    var input = Console.ReadLine();
    if (string.IsNullOrWhiteSpace(input) || input == Environment.NewLine)
    {
        continue;
    }

    try
    {
        chat.AddUserMessage(input);

        using var cts = new CancellationTokenSource();
        var result = await chat.SendAsync(chat.CreateChatMessages(), cts.Token);

        chat.AddAssistantMessage(result?.Message);

        Console.WriteLine(result?.Message);
    }
    catch (Exception ex)
    {
        Console.WriteLine("Error: " + ex.Message);
    }
}
