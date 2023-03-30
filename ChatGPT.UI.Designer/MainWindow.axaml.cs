using System;
using System.Threading;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using ChatGPT.ViewModels.Chat;

namespace ChatGPT;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
#if DEBUG
        this.AttachDevTools();
#endif
        Defaults.ConfigureDefaultServices();
    }

    private async void SendButton_OnClick(object? sender, RoutedEventArgs e)
    {
        SendButton.IsEnabled = false;
        PromptTextBox.IsEnabled = false;

        var prompt = PromptTextBox.Text;

        var directions = """
"You are Avalonia UI XAML design expert. 
I will write requirements for UI user control.
Avalonia UI xmlns is https://github.com/avaloniaui
Do not set x:Class.
Write XAML with UserControl as root. 
Do not write any explanation.
Do not format XAML with markdown code block.
Write XAML as plain text.
""";

        try
        {
            var chatSettings = new ChatSettingsViewModel
            {
                Temperature = 0.7m,
                TopP = 1m,
                PresencePenalty = 0m,
                FrequencyPenalty = 0m,
                MaxTokens = 2000,
                ApiKey = null,
                Model = "gpt-3.5-turbo",
                // Model = "gpt-4",
                Directions = directions,
            };

            var cts = new CancellationTokenSource();
            var chat = new ChatViewModel(chatSettings);
            chat.AddSystemMessage(chatSettings.Directions);
            chat.AddUserMessage(prompt);
            var result = await chat.SendAsync(chat.CreateChatMessages(), cts.Token);
            if (result?.Message is { } xaml)
            {
                Console.WriteLine(xaml);
                var control = AvaloniaRuntimeXamlLoader.Parse<Control?>(xaml, null);
                if (control is { })
                {
                    PreviewContentControl.Content = control;
                    XamlTextBox.Text = xaml;
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }

        SendButton.IsEnabled = true;
        PromptTextBox.IsEnabled = true;
    }
}
