using ChatGPT;
using ChatGPT.ViewModels.Chat;

Defaults.ConfigureDefaultServices();

using var cts = new CancellationTokenSource();
var input1 = "Some random test string.";
var directions1 = "You are a helpful assistant. Convert user message into hex string. Write only the output hex string. Do not write any additional text.";
var input2 = await SendAsync(directions1, input1, cts.Token);
Console.WriteLine(input2);
if (input2 is not null)
{
    var directions2 = "You are a helpful assistant. Convert user hex string into plain text. Write only the output plain text. Do not write any additional text.";
    var output = await SendAsync(directions2, input2, cts.Token);
    Console.WriteLine(output);
}

async Task<string?> SendAsync(string directions, string input, CancellationToken token)
{
    var chat = new ChatViewModel(directions);
    chat.AddSystemMessage(directions);
    chat.AddUserMessage(input);
    var result = await chat.SendAsync(chat.CreateChatMessages(), token);
    chat.AddAssistantMessage(result?.Message);
    return result?.Message;
}
