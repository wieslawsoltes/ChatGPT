using Avalonia;
using Avalonia.Controls;
using Avalonia.VisualTree;
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

    private SingleLayoutView Single = new SingleLayoutView();

    private ColumnLayoutView Column = new ColumnLayoutView();

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
                    Child = Single;
                    break;
                }
                case "Desktop":
                {
                    Child =  Column;
                    break;
                }
            }
        }
    }
}
