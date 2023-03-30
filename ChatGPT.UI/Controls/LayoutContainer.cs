using Avalonia;
using Avalonia.Controls;
using ChatGPT.Views.Layouts;

namespace ChatGPT.Controls;

public class LayoutContainer : Decorator
{
    public static readonly StyledProperty<string?> LayoutProperty = 
        AvaloniaProperty.Register<LayoutContainer, string?>(nameof(Layout));

    public string? Layout
    {
        get => GetValue(LayoutProperty);
        set => SetValue(LayoutProperty, value);
    }

    private readonly SingleLayoutView _single;
    private readonly ColumnLayoutView _column;

    public LayoutContainer()
    {
        _single = new SingleLayoutView();
        _column = new ColumnLayoutView();  
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == LayoutProperty)
        {
            var layout = change.GetNewValue<string?>();
            switch (layout)
            {
                case "Mobile":
                {
                    Child = _single;
                    break;
                }
                case "Desktop":
                {
                    Child = _column;
                    break;
                }
            }
        }
    }
}
