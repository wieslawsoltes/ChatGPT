using Android.App;
using Android.Content;
using Android.OS;
using Avalonia;
using Avalonia.Android;
using Avalonia.Fonts.Inter;
using Application = Android.App.Application;

namespace ChatGPT.UI.Android;

[Activity(
    Theme = "@style/MyTheme.Splash", 
    MainLauncher = true, 
    NoHistory = true)]
public class SplashActivity : AvaloniaSplashActivity<App>
{
    protected override AppBuilder CustomizeAppBuilder(AppBuilder builder)
    {
        App.ConfigureMobileServices();

        builder.WithInterFont();

        return base.CustomizeAppBuilder(builder);
    }

    protected override void OnCreate(Bundle? savedInstanceState)
    {
        base.OnCreate(savedInstanceState);
    }

    protected override void OnResume()
    {
        base.OnResume();

        StartActivity(new Intent(Application.Context, typeof(MainActivity)));

        Finish();
    }
}
