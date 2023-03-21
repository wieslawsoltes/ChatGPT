using AI.Model.Services;
using AI.Services;
using ChatGPT.Model.Services;
using ChatGPT.Services;
using ChatGPT.ViewModels.Chat;
using ChatGPT.ViewModels.Settings;
using CommunityToolkit.Mvvm.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

ConfigureServices();

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
        MaxTokens = 2000,
        Model = "gpt-3.5-turbo",
        Directions = directions
    }
};

chat.Messages.Add(new ChatMessageViewModel
{
    Role = "system",
    Message = directions,
    Format = chatSettings.Format,
});

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
        chat.Messages.Add(new ChatMessageViewModel
        {
            Role = "user",
            Message = input,
            Format = chatSettings.Format
        });

        var cts = new CancellationTokenSource();
        var messages = chat.CreateChatMessages();
        var result = await chat.Send(messages, cts.Token);

        chat.Messages.Add(new ChatMessageViewModel
        {
            Role = "assistant",
            Message = result?.Message,
            Format = chatSettings.Format
        });

        Console.WriteLine(result?.Message);
    }
    catch (Exception ex)
    {
        Console.WriteLine("Error: " + ex.Message);
    }
}

void ConfigureServices()
{
    IServiceCollection serviceCollection = new ServiceCollection();

    serviceCollection.AddSingleton<IStorageFactory, IsolatedStorageFactory>();
    serviceCollection.AddSingleton<IChatService, ChatService>();

    serviceCollection.AddTransient<ChatMessageViewModel>();
    serviceCollection.AddTransient<ChatSettingsViewModel>();
    serviceCollection.AddTransient<ChatResultViewModel>();
    serviceCollection.AddTransient<ChatViewModel>();
    serviceCollection.AddTransient<PromptViewModel>();

    Ioc.Default.ConfigureServices(serviceCollection.BuildServiceProvider());
}
