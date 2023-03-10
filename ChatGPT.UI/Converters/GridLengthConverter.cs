using System;
using System.Globalization;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data.Converters;

namespace ChatGPT.Converters;

public class GridLengthConverter : IValueConverter
{
    public static GridLengthConverter Instance = new();
    
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is string s)
        {
            return GridLength.Parse(s);
        }

        return AvaloniaProperty.UnsetValue;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is GridLength length)
        {
            return length.ToString();
        }

        return AvaloniaProperty.UnsetValue;
    }
}
