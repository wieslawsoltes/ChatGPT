using System.Runtime.Versioning;
using Avalonia;
using Avalonia.Browser;

[assembly:SupportedOSPlatform("browser")]

namespace ChatGPT.UI.Browser;

internal class Program
{
    private static void Main(string[] args) 
        => BuildAvaloniaApp().SetupBrowserApp("out");

    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>();
}
