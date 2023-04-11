using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using ChatGPT.ViewModels.Chat;

namespace ChatGPT;

public class Game
{
    private string? _directions;
    private CancellationTokenSource? _cts;
    private ChatViewModel? _chat;

    public void New()
    {
        _directions =
            """
Text based game in form of chat.

The initial prompt creates random text game. 

The game genre is random.

Player has always three options to choose.

Don't write that user has 3 options to choose. 

Assistant messages are as json:
{
    "story" : "The next part of the story",
    "option1" : "The option 1",
    "option2" : "The option 2",
    "option3" : "The option 3"
}
""";

        _chat = new ChatViewModel(new ChatSettingsViewModel
        {
            Temperature = 0.7m,
            TopP = 1m,
            PresencePenalty = 0m,
            FrequencyPenalty = 0m,
            MaxTokens = 3000,
            ApiKey = null,
            // Model = "gpt-3.5-turbo",
            Model = "gpt-4",
        });

        _chat.AddSystemMessage(_directions);
    }

    public async Task<GameMessage?> Send(string? input)
    {
        if (_chat is null)
        {
            return null;
        }

        _cts = new CancellationTokenSource();

        try
        {
            if (!string.IsNullOrEmpty(input))
            {
                _chat.AddUserMessage(input);
            }

            var result = await _chat.SendAsync(_chat.CreateChatMessages(), _cts.Token);
            var json = result?.Message;
            if (json is { })
            {
#if DEBUG
                Console.WriteLine(json);
#endif
                var gameMessage = JsonSerializer.Deserialize<GameMessage>(json);
                if (gameMessage is { })
                {
                    _chat.AddAssistantMessage(json);
                    return gameMessage;
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine("Error: " + e.Message);
        }

        return null;
    }
}
