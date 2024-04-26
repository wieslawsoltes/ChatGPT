using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using AI.Model.Services;
using AI.Services;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.Platform;
using Avalonia.Styling;
using ChatGPT.Model.Services;
using ChatGPT.Services;
using ChatGPT.ViewModels;
using ChatGPT.ViewModels.Layouts;
using ChatGPT.Views;

namespace ChatGPT;

public partial class App : Application
{
    // private IDisposable? _settingsDisposable;
    private readonly IChatSerializer _chatSerializer;
    private readonly IChatService _chatService;
    private readonly IStorageFactory _storageFactory;
    private readonly IApplicationService _applicationService;
    private readonly MainViewModel _mainViewModel;

    public App()
    {
        _chatSerializer = new SystemTextJsonChatSerializer();
        _chatService = new ChatService(_chatSerializer);

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
            || RuntimeInformation.IsOSPlatform(OSPlatform.OSX)
            || RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            _storageFactory = new ApplicationDataStorageFactory();
            _applicationService = new ApplicationService();
        }
        else if (IsOSPlatform("ANDROID")
                 || IsOSPlatform("IOS"))
        {
            _storageFactory = new IsolatedStorageFactory();
            _applicationService = new ApplicationService();
        }
        else if (IsOSPlatform("BROWSER"))
        {
            _storageFactory = new BrowserStorageFactory();
            _applicationService = new ApplicationService(); 
        }
        else
        {
            _storageFactory = new IsolatedStorageFactory();
            _applicationService = new ApplicationService();
        }

        _mainViewModel = new MainViewModel(
            _chatService,
            _chatSerializer,
            _applicationService,
            _storageFactory);
        
        static bool IsOSPlatform(string platform) 
            => RuntimeInformation.IsOSPlatform(OSPlatform.Create(platform));
    }

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override async void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            var mainWindow = new MainWindow
            {
                DataContext = _mainViewModel
            };
            desktop.MainWindow = mainWindow;

            mainWindow.Closing += MainWindowOnClosing;
            
            desktop.Exit += DesktopOnExit;

            await LoadWindowLayoutAsync(mainWindow);

            await InitSettingsAsync();
            SetTheme();
        }
        else if (ApplicationLifetime is ISingleViewApplicationLifetime single)
        {
            single.MainView = new MainView
            {
                DataContext = _mainViewModel
            };

            await InitSettingsAsync();
            SetTheme();
        }

        /*
        _settingsDisposable = DispatcherTimer.Run(
            () =>
            {
                Task.Run(async () => await SaveSettings());
                return true;
            }, 
            TimeSpan.FromSeconds(5));
        */

        base.OnFrameworkInitializationCompleted();
    }

    private async Task InitSettingsAsync()
    {
        try
        {
            await LoadSettingsAsync();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }

    private void SetTheme()
    {
        if (_mainViewModel?.Theme is { } theme)
        {
            switch (theme)
            {
                case "Light":
                    RequestedThemeVariant = ThemeVariant.Light;
                    break;
                case "Dark":
                    RequestedThemeVariant = ThemeVariant.Dark;
                    break;
            }
        }
    }

    public async Task LoadWindowLayoutAsync(Window window)
    {
        var storage = _storageFactory?.CreateStorageService<WindowLayoutViewModel>();
        if (storage is null)
        {
            return;
        }
        var layout = await storage.LoadObjectAsync(
            "WindowLayout", 
            WindowLayoutViewModelJsonContext.s_instance.WindowLayoutViewModel);
        if (layout is { })
        {
            window.Position = new PixelPoint(layout.X, layout.Y);

            window.Width = layout.Width;

            window.Height = layout.Height;

            if (layout.WindowState is { })
            {
                if (Enum.TryParse<WindowState>(layout.WindowState, out var windowState))
                {
                    window.WindowState = windowState;
                }
            }

            if (layout.WindowStartupLocation is { })
            {
                if (Enum.TryParse<WindowState>(layout.WindowState, out var windowState))
                {
                    window.WindowState = windowState;
                }
            }

            window.Topmost = layout.Topmost;
        }
    }

    public async Task SaveWindowLayoutAsync(Window window)
    {
        var workspace = new WindowLayoutViewModel
        {
            X = window.Position.X,
            Y = window.Position.Y,
            Width = window.Width,
            Height = window.Height,
            WindowState = window.WindowState.ToString(),
            WindowStartupLocation = WindowStartupLocation.Manual.ToString(),
            Topmost = window.Topmost
        };

        var storage = _storageFactory?.CreateStorageService<WindowLayoutViewModel>();
        if (storage is { })
        {
            await storage.SaveObjectAsync(
                workspace, 
                "WindowLayout", 
                WindowLayoutViewModelJsonContext.s_instance.WindowLayoutViewModel);
        }
    }

    private async void MainWindowOnClosing(object? sender, WindowClosingEventArgs e)
    {
        if (sender is Window window)
        {
            await SaveWindowLayoutAsync(window);
        }
    }

    private void DesktopOnExit(object? sender, ControlledApplicationLifetimeExitEventArgs e)
    {
        try
        {
            // _settingsDisposable?.Dispose();

            SaveTheme();
            SaveSettings();
        }
        catch (Exception exception)
        {
            Console.WriteLine(exception);
        }
    }

    public async Task LoadSettingsAsync()
    {
        if (_mainViewModel is null)
        {
            return;
        }
        await _mainViewModel.LoadSettingsAsync();
    }

    public async Task SaveSettingsAsync()
    {
        if (_mainViewModel is null)
        {
            return;
        }

        await _mainViewModel.SaveSettingsAsync();
    }

    public void LoadSettings()
    {
        if (_mainViewModel is null)
        {
            return;
        }

        _mainViewModel.LoadSettings();
    }

    public void SaveSettings()
    {
        if (_mainViewModel is null)
        {
            return;
        }

        _mainViewModel.SaveSettings();
    }

    public void SaveTheme()
    {
        var theme = "Light";
        if (RequestedThemeVariant == ThemeVariant.Dark)
        {
            theme = "Dark";
        }

        if (_mainViewModel is { })
        {
            _mainViewModel.Theme = theme;
        }
    }

    public void ToggleAcrylicBlur()
    {
        if (Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            if (desktop.MainWindow is MainWindow mainWindow)
            {
                if (mainWindow.TransparencyLevelHint.Contains(WindowTransparencyLevel.AcrylicBlur))
                {
                    mainWindow.SystemDecorations = SystemDecorations.None;
                    mainWindow.ExtendClientAreaChromeHints = ExtendClientAreaChromeHints.NoChrome;
                    mainWindow.ExtendClientAreaToDecorationsHint = false;
                    mainWindow.TransparencyLevelHint = new [] { WindowTransparencyLevel.Transparent };
                    mainWindow.AcrylicBorder.IsVisible = false;
                }
                else
                {
                    mainWindow.SystemDecorations = SystemDecorations.Full;
                    mainWindow.ExtendClientAreaChromeHints = ExtendClientAreaChromeHints.PreferSystemChrome;
                    mainWindow.ExtendClientAreaToDecorationsHint = true;
                    mainWindow.TransparencyLevelHint = new []{ WindowTransparencyLevel.AcrylicBlur };
                    mainWindow.AcrylicBorder.IsVisible = true;
                }
            }
        }
    }

    public void ToggleWindowState()
    {
        if (Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            if (desktop.MainWindow is MainWindow mainWindow)
            {
                if (mainWindow.WindowState == WindowState.Minimized)
                {
                    mainWindow.WindowState = WindowState.Normal;
                    mainWindow.ShowInTaskbar = true;
                    mainWindow.Show();
                }
                else
                {
                    mainWindow.WindowState = WindowState.Minimized;
                    mainWindow.ShowInTaskbar = false;
                    mainWindow.Hide();
                }
            }
        }
    }

    private void TrayIcon_OnClicked(object? sender, EventArgs e)
    {
        ToggleWindowState();
    }

    private void ToggleShow_OnClick(object? sender, EventArgs e)
    {
        ToggleWindowState();
    }

    private void ToggleAcrylic_OnClick(object? sender, EventArgs e)
    {
        ToggleAcrylicBlur();
    }

    private void Quit_OnClick(object? sender, EventArgs e)
    {
        if (_applicationService is { })
        {
            _applicationService.Exit();
        }
    }
}
