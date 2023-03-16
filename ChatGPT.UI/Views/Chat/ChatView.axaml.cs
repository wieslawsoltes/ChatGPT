using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Reactive;

namespace ChatGPT.Views.Chat;

public partial class ChatView : UserControl
{
    private bool _scrollToEnd;
    
    public ChatView()
    {
        InitializeComponent();

        MessagesItemsControl
            .GetObservable(SelectingItemsControl.SelectedItemProperty)
            .Subscribe(new AnonymousObserver<object?>(x =>
            {
                _scrollToEnd = true;
            }));

        ChatScrollViewer
            .GetObservable(ScrollViewer.ExtentProperty)
            .Subscribe(new AnonymousObserver<Size>(x =>
            {
                if (_scrollToEnd)
                {
                    ChatScrollViewer.ScrollToEnd();
                    _scrollToEnd = false;
                }
            }));
    }
}
