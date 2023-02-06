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
        if (SendGrid.IsEnabled && e.Key is Key.Enter or Key.F2)
        {
            if (Send.Command?.CanExecute("Edit") ?? false)
            {
                Send.Command?.Execute("Edit");
            }
        }

        if (SendGrid.IsEnabled && e.Key == Key.Escape)
        {
            if (Edit.Command?.CanExecute("Cancel") ?? false)
            {
                Edit.Command?.Execute("Cancel");
            }
        }
    }
}
