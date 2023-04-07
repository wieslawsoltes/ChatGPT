using ChatGPT;
using ChatGPT.ViewModels.Chat;

Defaults.ConfigureDefaultServices();

var directions = 
"""
Text based game in form of chat.

The initial prompt creates random text game. 

Player has always three options to choose.

Never write that user has 3 options to choose. 

Assistant messages are as json:
{
    "story" : "The next part of the story",
    "option1" : "The option 1",
    "option2" : "The option 2",
    "option3" : "The option 3",
}
""";

if (args.Length == 1)
{
    directions = args[0];
}

using var cts = new CancellationTokenSource();

var chat = new ChatViewModel(new ChatSettingsViewModel
{
    MaxTokens = 4000,
    Model = "gpt-4"
});

chat.AddSystemMessage(directions);

await SendAsync(chat, null, cts);

while (true)
{
    Console.Write("> ");

    var input = Console.ReadLine();
    if (string.IsNullOrWhiteSpace(input) || input == Environment.NewLine)
    {
        continue;
    }

    await SendAsync(chat, input, cts);
}

static async Task SendAsync(ChatViewModel chat, string? message, CancellationTokenSource cts)
{
    try
    {
        if (!string.IsNullOrEmpty(message))
        {
            chat.AddUserMessage(message);
        }
        var result = await chat.SendAsync(chat.CreateChatMessages(), cts.Token);
        chat.AddAssistantMessage(result?.Message);
        Console.WriteLine(result?.Message);
    }
    catch (Exception ex)
    {
        Console.WriteLine("Error: " + ex.Message);
    }
}
