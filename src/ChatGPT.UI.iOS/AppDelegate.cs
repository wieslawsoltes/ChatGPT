using Avalonia;
using Avalonia.Fonts.Inter;
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
        App.ConfigureMobileServices();

        builder.WithInterFont();

        return base.CustomizeAppBuilder(builder);
    }
}
