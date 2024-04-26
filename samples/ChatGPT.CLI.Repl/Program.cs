using AI.Services;
using ChatGPT.ViewModels.Chat;

var chatSerializer = new SystemTextJsonChatSerializer();
var chatService = new ChatService(chatSerializer);

var directions = 
"""
You are a helpful assistant.
Write answers in plain text.
Do not use markdown.
""";

if (args.Length == 1)
{
    directions = args[0];
}

using var cts = new CancellationTokenSource();

var chat = new ChatViewModel(
    chatService,
    chatSerializer,
    new ChatSettingsViewModel
    {
        MaxTokens = 2000,
        Model = "gpt-3.5-turbo"
    });

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
        var result = await chat.SendAsync(chat.CreateChatMessages(), cts.Token);
        chat.AddAssistantMessage(result?.Message);
        Console.WriteLine(result?.Message);
    }
    catch (Exception ex)
    {
        Console.WriteLine("Error: " + ex.Message);
    }
}
