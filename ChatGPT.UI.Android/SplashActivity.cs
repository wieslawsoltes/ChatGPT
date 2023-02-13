using System;
using Android.App;
using Android.Content;
using Avalonia.Android;

namespace ChatGPT.UI.Android;

[Activity(Theme = "@style/MyTheme.Splash", MainLauncher = true, NoHistory = true)]
public class SplashActivity : AvaloniaSplashActivity<App>
{
    protected override void OnResume()
    {
        base.OnResume();

        StartActivity(new Intent(Application.Context, typeof(MainActivity)));
        Finish();
    }

    protected override async void OnDestroy()
    {
        base.OnDestroy();

        if (Avalonia.Application.Current is App app)
        {
            try
            {
                await app.SaveSettings();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }
    }
}
