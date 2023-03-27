using ChatGPT;
using ChatGPT.ViewModels.Chat;

Defaults.ConfigureDefaultServices();

var input1 = "You are a helpful assistant.";
var directions1 = "You are a helpful assistant. Convert user message into hex string. Write only the output hex string. Do not write any additional text.";
var input2 = await SendAsync(directions1, input1);
Console.WriteLine(input2);
if (input2 is not null)
{
    var directions2 = "You are a helpful assistant. Convert user hex string into plain text. Write only the output plain text. Do not write any additional text.";
    var output = await SendAsync(directions2, input2);
    Console.WriteLine(output);
}

async Task<string?> SendAsync(string directions, string input)
{
    var chat = new ChatViewModel
    {
        Settings = new ChatSettingsViewModel
        {
            MaxTokens = 2000,
            Model = "gpt-3.5-turbo",
            Directions = directions
        }
    };
    chat.AddSystemMessage(directions);
    chat.AddUserMessage(input);
    using var cts = new CancellationTokenSource();
    var result = await chat.SendAsync(chat.CreateChatMessages(), cts.Token);
    chat.AddAssistantMessage(result?.Message);
    return result?.Message;
}
