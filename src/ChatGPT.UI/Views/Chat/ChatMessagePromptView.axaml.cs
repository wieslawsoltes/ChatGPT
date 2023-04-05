using Avalonia.Controls;
using Avalonia.Input;

namespace ChatGPT.Views.Chat;

public partial class ChatMessagePromptView : UserControl
{
    public ChatMessagePromptView()
    {
        InitializeComponent();
    }

    public void InsertNewLine()
    {
        var args = new TextInputEventArgs
        {
            Text = Prompt.NewLine,
            RoutedEvent = InputElement.TextInputEvent
        };

        Prompt.RaiseEvent(args);
    }
}
