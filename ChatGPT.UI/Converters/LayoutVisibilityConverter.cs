using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace ChatGPT.Converters;

public class LayoutVisibilityConverter : IValueConverter
{
    public static LayoutVisibilityConverter Instance = new();

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is string layout && parameter is string targetLayout)
        {
            return layout == targetLayout;
        }

        return false;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
