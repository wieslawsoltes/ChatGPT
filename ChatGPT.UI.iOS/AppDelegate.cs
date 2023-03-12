/*
 * SPDX-License-Identifier: AGPL-3.0-or-later
 * Copyright (C) 2022-2023, Wiesław Šoltés. All rights reserved.
 */
using Avalonia;
using Avalonia.iOS;
using Foundation;

namespace ChatGPT.UI.iOS;

// The UIApplicationDelegate for the application. This class is responsible for launching the 
// User Interface of the application, as well as listening (and optionally responding) to 
// application events from iOS.
[Register("AppDelegate")]
public class AppDelegate : AvaloniaAppDelegate<App>
{
    protected override AppBuilder CustomizeAppBuilder(AppBuilder builder)
    {
        App.ConfigureDefaultServices();

        return base.CustomizeAppBuilder(builder);
    }
}
