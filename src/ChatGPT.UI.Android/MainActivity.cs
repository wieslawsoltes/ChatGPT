using System;
using Android.App;
using Android.Content.PM;
using Avalonia.Android;

namespace ChatGPT.UI.Android;

[Activity(
    Label = "ChatGPT", 
    Theme = "@style/MyTheme.NoActionBar", 
    Icon = "@drawable/icon", 
    LaunchMode = LaunchMode.SingleInstance, 
    ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.ScreenSize)]
public class MainActivity : AvaloniaMainActivity
{
    protected override void OnDestroy()
    {
        base.OnDestroy();

        if (Avalonia.Application.Current is App app)
        {
            try
            {
                app.SaveTheme();
                app.SaveSettings();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }
    }
}
