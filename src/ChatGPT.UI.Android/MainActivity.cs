using System;
using Android.App;
using Android.Content.PM;
using Android.Views;
using Avalonia;
using Avalonia.Android;

namespace ChatGPT.UI.Android;

[Activity(
    Label = "ChatGPT", 
    Theme = "@style/MyTheme.Main", 
    Icon = "@drawable/icon",
    MainLauncher = true,
    LaunchMode = LaunchMode.SingleTop, 
    ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.ScreenSize | ConfigChanges.UiMode,
    WindowSoftInputMode = SoftInput.AdjustResize)]
public class MainActivity : AvaloniaMainActivity<App>
{
    protected override void OnDestroy()
    {
        try
        {
            if (Avalonia.Application.Current is App app)
            {
                app.SaveTheme();
                app.SaveSettings();
            }
        }
        catch (Exception exception)
        {
            Console.WriteLine(exception);
        }

        base.OnDestroy();
    }
    
    protected override AppBuilder CustomizeAppBuilder(AppBuilder builder)
    {
        builder.WithInterFont();

        return base.CustomizeAppBuilder(builder);
    }
}
