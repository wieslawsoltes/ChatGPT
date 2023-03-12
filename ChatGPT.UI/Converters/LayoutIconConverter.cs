/*
 * SPDX-License-Identifier: AGPL-3.0-or-later
 * Copyright (C) 2022-2023, Wiesław Šoltés. All rights reserved.
 */
using System;
using System.Globalization;
using Avalonia;
using Avalonia.Data.Converters;

namespace ChatGPT.Converters;

public class LayoutIconConverter : IValueConverter
{
    public static LayoutIconConverter Instance = new();

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is string layout)
        {
            var icon = layout switch
            {
                "Mobile" => "ViewDesktopIcon",
                "Desktop" => "ViewDesktopMobileIcon",
                _ => default
            };

            if (icon is null)
            {
                return AvaloniaProperty.UnsetValue;
            }

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
