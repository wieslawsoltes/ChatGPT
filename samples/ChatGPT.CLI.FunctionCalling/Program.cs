using ChatGPT;
using ChatGPT.ViewModels.Chat;

Defaults.ConfigureDefaultServices();

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

var functions = GetFunctions();

var chat = new ChatViewModel(new ChatSettingsViewModel
{
    MaxTokens = 2000,
    Model = "gpt-3.5-turbo-0613",
    Functions = functions,
    FunctionCall = "auto"
    // Force function call by setting FunctionCall property.
    // FunctionCall = new { name = "GetCurrentWeather" }
});

// Enable to debug json requests and responses.
// chat.Debug = true;

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

        if (result?.Message is { })
        {
            Console.WriteLine(result.Message);
        }

        if (result?.FunctionCall is { } functionCall)
        {
            if (functionCall.Name == "GetCurrentWeather" && functionCall.Arguments is { })
            {
                functionCall.Arguments.TryGetValue("location", out var location);
                functionCall.Arguments.TryGetValue("unit", out var unit);
                var functionCallResult = GetCurrentWeather(location, unit ?? "celsius");
                chat.AddFunctionMessage(functionCallResult, functionCall.Name);

                Console.WriteLine(functionCallResult);
            }
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine("Error: " + ex.Message);
    }
}

string GetCurrentWeather(string? location, string? unit)
{
    Console.WriteLine($"Weather for {location} [{unit}].");
    return "Cloudy.";
}

object GetFunctions()
{
    return new[]
    {
        new
        {
            name = "GetCurrentWeather",
            description = "Get the current weather in a given location",
            parameters = new
            {
                type = "object",
                properties = new
                {
                    location = new
                    {
                        type = "string",
                        description = "The city and state, e.g. San Francisco, CA"
                    },
                    unit = new
                    {
                        type = "string",
                        @enum = new[] {"celsius", "fahrenheit"}
                    },
                },
                required = new[] {"location"}
            },
        }
    };
}
