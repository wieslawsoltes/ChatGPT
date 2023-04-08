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
            .Subscribe(new AnonymousObserver<object?>(_ =>
            {
                _scrollToEnd = true;
            }));

        ChatScrollViewer
            .GetObservable(ScrollViewer.ExtentProperty)
            .Subscribe(new AnonymousObserver<Size>(_ =>
            {
                if (_scrollToEnd)
                {
                    ChatScrollViewer.ScrollToEnd();
                    _scrollToEnd = false;
                }
            }));

        this.GetObservable(BoundsProperty)
            .Subscribe(new AnonymousObserver<Rect>(_ =>
            {
                ChatScrollViewer.ScrollToEnd();
            }));
    }
}
