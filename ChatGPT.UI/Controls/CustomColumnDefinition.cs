using System;
using Avalonia;
using Avalonia.Controls;

namespace ChatGPT.Controls;

public class CustomColumnDefinition : ColumnDefinition
{
    public static readonly StyledProperty<bool> IsVisibleProperty =
        AvaloniaProperty.Register<ColumnDefinition, bool>(nameof(IsVisible), false);

    public static readonly StyledProperty<GridLength> BindWidthProperty =
        AvaloniaProperty.Register<ColumnDefinition, GridLength>(nameof(BindWidth), new GridLength(1, GridUnitType.Star));

    private IDisposable? _disposable1;
    private IDisposable? _disposable2;

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

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == IsVisibleProperty)
        {
            var isVisible = change.GetNewValue<bool>();
            if (isVisible)
            {
                Show();
            }
            else
            {
                Hide();
            }
        }

        if (change.Property == BindWidthProperty)
        {
            // TODO:
            Width = change.GetNewValue<GridLength>();
        }
    }

    private void Show()
    {
        if (_disposable1 is null && _disposable2 is null)
        {
            // TODO:
            Width = BindWidth;
            // _disposable1 = Bind(WidthProperty, this.GetObservable(BindWidthProperty));
            // _disposable2 = Bind(BindWidthProperty, this.GetObservable(WidthProperty));
        }
    }

    private void Hide()
    {
        _disposable2?.Dispose();
        _disposable2 = null;
        _disposable1?.Dispose();
        _disposable1 = null;

        // TODO:
        BindWidth = Width;
        Width = new GridLength(0, GridUnitType.Pixel);
    }
}
