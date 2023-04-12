using Avalonia;
using Avalonia.Controls;

namespace ChatGPT.Views;

public partial class HeaderView : UserControl
{
    public static readonly StyledProperty<string?> HeaderProperty =
        AvaloniaProperty.Register<HeaderView, string?>(nameof(Header));

    public string? Header
    {
        get => GetValue(HeaderProperty);
        set => SetValue(HeaderProperty, value);
    }

    public HeaderView()
    {
        InitializeComponent();
        DataContext = this;
    }
}
