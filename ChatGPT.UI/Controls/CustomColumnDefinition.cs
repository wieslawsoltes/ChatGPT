using Avalonia;
using Avalonia.Controls;

namespace ChatGPT.Controls;

public class CustomColumnDefinition : ColumnDefinition
{
    public static readonly StyledProperty<bool> IsVisibleProperty =
        AvaloniaProperty.Register<ColumnDefinition, bool>(nameof(IsVisible));

    public static readonly StyledProperty<GridLength> BindWidthProperty =
        AvaloniaProperty.Register<ColumnDefinition, GridLength>(nameof(BindWidth), new GridLength(1, GridUnitType.Star));

    public bool IsVisible
    {
        get => GetValue(IsVisibleProperty);
        set => SetValue(IsVisibleProperty, value);
    }
    
    public GridLength BindWidth
    {
        get => GetValue(BindWidthProperty);
        set => SetValue(BindWidthProperty, value);
    }

    public CustomColumnDefinition()
    {
        ToggleIsVisible(IsVisible);
    }

    private void ToggleIsVisible(bool isVisible)
    {
        if (isVisible)
        {
            Show();
        }
        else
        {
            Hide();
        }
    }
    
    private void UpdateWidth(GridLength bindWidth)
    {
        if (IsVisible)
        {
            Width = bindWidth;
        }
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == IsVisibleProperty)
        {
            ToggleIsVisible(change.GetNewValue<bool>());
        }

        if (change.Property == BindWidthProperty)
        {
            UpdateWidth(change.GetNewValue<GridLength>());
        }
    }

    private void Show()
    {
        Width = BindWidth;
    }

    private void Hide()
    {
        Width = new GridLength(0, GridUnitType.Pixel);
    }
}
