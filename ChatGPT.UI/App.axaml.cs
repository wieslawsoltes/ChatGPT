using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using ChatGPT.UI.Model.Services;
using ChatGPT.UI.Services;
using ChatGPT.UI.ViewModels;
using ChatGPT.UI.Views;
using CommunityToolkit.Mvvm.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace ChatGPT.UI;

public partial class App : Application
{
    public App()
    {
        ConfigureServices();
    }

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow
            {
                DataContext = new MainViewModel()
            };
        }
        else if (ApplicationLifetime is ISingleViewApplicationLifetime single)
        {
            single.MainView = new MainView
            {
                DataContext = new MainViewModel()
            };
        }

        base.OnFrameworkInitializationCompleted();
    }

    private void ConfigureServices()
    {
        Ioc.Default.ConfigureServices(
            new ServiceCollection()
                // Services
                .AddSingleton<IApplicationService, ApplicationService>()
                .AddSingleton<IChatService, ChatService>()
                // ViewModels
                .AddTransient<ActionsViewModel>()
                .AddTransient<SettingsViewModel>()
                .AddTransient<MessageViewModel>()
                .AddTransient<MainViewModel>()
                .BuildServiceProvider());
    }
}
