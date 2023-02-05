using Avalonia.Controls;
using Avalonia.Input;

namespace ChatGPT.UI.Views;

public partial class MessageView : UserControl
{
    public MessageView()
    {
        InitializeComponent();
        
        Prompt.KeyDown += PromptOnKeyDown;
    }

    private void PromptOnKeyDown(object? sender, KeyEventArgs e)
    {
        if (Grid.IsEnabled && e.Key == Key.Enter)
        {
            if (Send.Command?.CanExecute(null) ?? false)
            {
                Send.Command?.Execute(null);
            }
        }
    }
}
