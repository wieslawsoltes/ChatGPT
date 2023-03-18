using System.Runtime.Versioning;
using AI.Model.Services;
using AI.Services;
using Avalonia;
using Avalonia.Browser;
using ChatGPT.Model.Plugins;
using ChatGPT.Model.Services;
using ChatGPT.Services;
using ChatGPT.UI.Browser.Services;
using ChatGPT.ViewModels;
using ChatGPT.ViewModels.Chat;
using ChatGPT.ViewModels.Layouts;
using ChatGPT.ViewModels.Settings;
using CommunityToolkit.Mvvm.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

[assembly:SupportedOSPlatform("browser")]

namespace ChatGPT.UI.Browser;

internal class Program
{
    private static void Main(string[] args) 
        => BuildAvaloniaApp().SetupBrowserApp("out");

    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder
            .Configure<App>()
            .AfterSetup(_ =>
            {
                ConfigureDefaultServices();
            });

    private static void ConfigureDefaultServices()
    {
        IServiceCollection serviceCollection = new ServiceCollection();

        // Services
        serviceCollection.AddSingleton<IStorageFactory, BrowserStorageFactory>();
        serviceCollection.AddSingleton<IApplicationService, ApplicationService>();
        serviceCollection.AddSingleton<IPluginsService, PluginsService>();
        serviceCollection.AddSingleton<IChatService, ChatService>();
        serviceCollection.AddSingleton<ICompletionsService, CompletionsService>();
        serviceCollection.AddSingleton<MainViewModel>();
        serviceCollection.AddSingleton<IPluginContext>(x => x.GetRequiredService<MainViewModel>());

        // ViewModels
        serviceCollection.AddTransient<ChatMessageViewModel>();
        serviceCollection.AddTransient<ChatSettingsViewModel>();
        serviceCollection.AddTransient<ChatViewModel>();
        serviceCollection.AddTransient<PromptViewModel>();
        serviceCollection.AddTransient<WorkspaceViewModel>();
        serviceCollection.AddTransient<WindowLayoutViewModel>();

        Ioc.Default.ConfigureServices(serviceCollection.BuildServiceProvider());
    }
}
