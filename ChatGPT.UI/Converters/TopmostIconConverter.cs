using System;
using System.Globalization;
using Avalonia;
using Avalonia.Data.Converters;

namespace ChatGPT.Converters;

public class TopmostIconConverter : IValueConverter
{
    public static TopmostIconConverter Instance = new();

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is bool topmost)
        {
            var icon = topmost switch
            {
                true => "PinIcon",
                false => "PinOffIcon"
            };

            if (Application.Current?.Resources.TryGetResource(icon, null, out var resource) == true)
            {
                return resource;
            }
        }

        return AvaloniaProperty.UnsetValue;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
