using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Styling;

namespace ChatGPT.Controls;

public class InvalidateContentControl : ContentControl, IStyleable
{
    public static readonly StyledProperty<object?> InvalidateTriggerProperty = 
        AvaloniaProperty.Register<InvalidateContentControl, object?>(nameof(InvalidateTrigger));

    public object? InvalidateTrigger
    {
        get => GetValue(InvalidateTriggerProperty);
        set => SetValue(InvalidateTriggerProperty, value);
    }

    Type IStyleable.StyleKey => typeof(ContentControl);

    private bool _firstTrigger = true;

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == InvalidateTriggerProperty)
        {
            if (_firstTrigger)
            {
                _firstTrigger = false;
            }
            else
            {
                var content = Content;
                Content = null;
                Content = content;
            }
        }
    }
}
