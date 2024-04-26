using System.Runtime.Versioning;
using System.Threading.Tasks;
using AI.Model.Services;
using AI.Services;
using Avalonia;
using Avalonia.Browser;
using ChatGPT.Model.Services;
using ChatGPT.Services;
using ChatGPT.ViewModels;
using ChatGPT.ViewModels.Chat;
using ChatGPT.ViewModels.Layouts;
using ChatGPT.ViewModels.Settings;
using CommunityToolkit.Mvvm.DependencyInjection;

[assembly:SupportedOSPlatform("browser")]

namespace ChatGPT.UI.Browser;

internal class Program
{
    private static async Task Main(string[] args) 
        => await BuildAvaloniaApp().StartBrowserAppAsync("out");

    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder
            .Configure<App>()
            .WithInterFont();
}
