using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using ChatGPT.ViewModels.Chat;

namespace ChatGPT;

public class GameMessage
{
    [JsonPropertyName("story")]
    public string? Story { get; set; }

    [JsonPropertyName("option1")]
    public string? Option1 { get; set; }

    [JsonPropertyName("option2")]
    public string? Option2 { get; set; }

    [JsonPropertyName("option3")]
    public string? Option3 { get; set; }
}

public partial class MainWindow : Window
{
    private string _directions;
    private CancellationTokenSource _cts;
    private ChatViewModel _chat;

    public MainWindow()
    {
        InitializeComponent();
#if DEBUG
        this.AttachDevTools();
#endif
        Defaults.ConfigureDefaultServices();
        
        New();
        
        Loaded += OnLoaded;
    }

    private async void OnLoaded(object? sender, RoutedEventArgs e)
    {
        await Send(null);
    }

    private void New()
    {
        _directions =
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
            //Model = "gpt-3.5-turbo",
            Model = "gpt-4",
        });

        _chat.AddSystemMessage(_directions);
    }

    private async Task Send(string? input)
    {
        Option1Button.IsEnabled = false;
        Option2Button.IsEnabled = false;
        Option3Button.IsEnabled = false;

        _cts = new CancellationTokenSource();

        try
        {
            var json = await SendAsync(_chat, input, _cts);
            if (json is { })
            {
                var gameResult = JsonSerializer.Deserialize<GameMessage>(json);
                if (gameResult is { })
                {
                    StoryTextBlock.Text = gameResult.Story;
                    Option1TextBlock.Text = gameResult.Option1;
                    Option2TextBlock.Text = gameResult.Option2;
                    Option3TextBlock.Text = gameResult.Option3;
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }

        Option1Button.IsEnabled = true;
        Option2Button.IsEnabled = true;
        Option3Button.IsEnabled = true;
    }

    private async Task<string?> SendAsync(ChatViewModel chat, string? message, CancellationTokenSource cts)
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
            return result?.Message;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex.Message);
        }

        return null;
    }

    private async void Option1Button_OnClick(object? sender, RoutedEventArgs e)
    {
        await Send("1");
    }

    private async void Option2Button_OnClick(object? sender, RoutedEventArgs e)
    {
        await Send("2");
    }

    private async void Option3Button_OnClick(object? sender, RoutedEventArgs e)
    {
        await Send("3");
    }
}
