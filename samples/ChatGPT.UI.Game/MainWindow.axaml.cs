using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace ChatGPT;

public partial class MainWindow : Window
{
    private readonly Game _game;

    public MainWindow()
    {
        InitializeComponent();
#if DEBUG
        this.AttachDevTools();
#endif
        Defaults.ConfigureDefaultServices();

        _game = new Game();
        _game.New();

        Loaded += async (_, _) => await Send(null);

        Option1Button.Click += async (_, _) => await Send("1");

        Option2Button.Click += async (_, _) => await Send("2");

        Option3Button.Click += async (_, _) => await Send("3");

        NewGameButton.Click += async (_, _) =>
        {
            _game.New();
            await Send(null);
        };

        LoadGameButton.Click += async (_, _) =>
        {
            // TODO:
        };

        SaveGameButton.Click += async (_, _) =>
        {
            // TODO:
        };
    }

    private async Task Send(string? input)
    {
        Option1Button.IsEnabled = false;
        Option2Button.IsEnabled = false;
        Option3Button.IsEnabled = false;
        NewGameButton.IsEnabled = false;
        LoadGameButton.IsEnabled = false;
        SaveGameButton.IsEnabled = false;

        var gameMessage = await _game.Send(input);
        if (gameMessage is { })
        {
            StoryTextBlock.Text = gameMessage.Story;
            Option1TextBlock.Text = gameMessage.Option1;
            Option2TextBlock.Text = gameMessage.Option2;
            Option3TextBlock.Text = gameMessage.Option3;
        }

        Option1Button.IsEnabled = true;
        Option2Button.IsEnabled = true;
        Option3Button.IsEnabled = true;
        NewGameButton.IsEnabled = true;
        LoadGameButton.IsEnabled = true;
        SaveGameButton.IsEnabled = true;
    }
}
