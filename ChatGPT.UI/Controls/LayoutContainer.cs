using Avalonia;
using Avalonia.Controls;
using Avalonia.VisualTree;
using ChatGPT.Views.Layouts;

namespace ChatGPT.Controls;

public class LayoutContainer : Decorator
{
    public static readonly StyledProperty<string?> LayoutProperty = 
        AvaloniaProperty.Register<LayoutContainer, string?>(nameof(Layout));

    public static readonly StyledProperty<double> LayoutWidthProperty = 
        AvaloniaProperty.Register<LayoutContainer, double>(nameof(LayoutWidth));

    public static readonly StyledProperty<double> LayoutHeightProperty = 
        AvaloniaProperty.Register<LayoutContainer, double>(nameof(LayoutHeight));

    public string? Layout
    {
        get => GetValue(LayoutProperty);
        set => SetValue(LayoutProperty, value);
    }

    public double LayoutWidth
    {
        get => GetValue(LayoutWidthProperty);
        set => SetValue(LayoutWidthProperty, value);
    }

    public double LayoutHeight
    {
        get => GetValue(LayoutHeightProperty);
        set => SetValue(LayoutHeightProperty, value);
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

        if (change.Property == LayoutWidthProperty)
        {
            var width = change.GetNewValue<double>();
            if (this.GetVisualRoot() is TopLevel topLevel)
            {
                // TODO:
                // topLevel.Width = width;
            }
        }

        if (change.Property == LayoutHeightProperty)
        {
            var height = change.GetNewValue<double>();
            if (this.GetVisualRoot() is TopLevel topLevel)
            {
                // TODO:
                // topLevel.Height = height;
            }
        }
    }
}
