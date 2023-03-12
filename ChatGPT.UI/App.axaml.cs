/*
 * SPDX-License-Identifier: AGPL-3.0-or-later
 * Copyright (C) 2022-2023, Wiesław Šoltés. All rights reserved.
 */
using System;
using System.IO;
using System.Threading.Tasks;
using AI.Model.Services;
using AI.Services;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.Platform;
using Avalonia.Styling;
using ChatGPT.Model.Plugins;
using ChatGPT.Model.Services;
using ChatGPT.Services;
using ChatGPT.ViewModels;
using ChatGPT.ViewModels.Chat;
using ChatGPT.ViewModels.Settings;
using ChatGPT.Views;
using CommunityToolkit.Mvvm.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace ChatGPT;

public partial class App : Application
{
    private const string SettingsFolderName = "ChatGPT";

    // TODO: Remove usage of SettingsFileName.
    private const string SettingsFileName = "settings.json";

    public App()
    {
    }

    public static void ConfigureDefaultServices()
    {
        IServiceCollection serviceCollection = new ServiceCollection();

        // Services
        serviceCollection.AddSingleton<IStorageFactory, IsolatedStorageFactory>();
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

        Ioc.Default.ConfigureServices(serviceCollection.BuildServiceProvider());
    }

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override async void OnFrameworkInitializationCompleted()
    {
        Ioc.Default.GetService<IPluginsService>()?.DiscoverPlugins();

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow
            {
                DataContext = Ioc.Default.GetService<MainViewModel>()
            };

            desktop.Exit += DesktopOnExit;

            await InitSettings();
            SetTheme();

            // TODO: Enable plugins.
            Ioc.Default.GetService<IPluginsService>()?.InitPlugins();
            // Ioc.Default.GetService<IPluginsService>()?.StartPlugins();
        }
        else if (ApplicationLifetime is ISingleViewApplicationLifetime single)
        {
            single.MainView = new MainView
            {
                DataContext = Ioc.Default.GetService<MainViewModel>()
            };

            await InitSettings();
            SetTheme();
        }

        base.OnFrameworkInitializationCompleted();
    }

    private async Task InitSettings()
    {
        try
        {
            await LoadSettings();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }

    private void SetTheme()
    {
        var mainViewModel = Ioc.Default.GetService<MainViewModel>();
        if (mainViewModel?.Theme is { } theme)
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

    private async void DesktopOnExit(object? sender, ControlledApplicationLifetimeExitEventArgs e)
    {
        try
        {
            Ioc.Default.GetService<IPluginsService>()?.ShutdownPlugins();

            SaveTheme();
            await SaveSettings();
        }
        catch (Exception exception)
        {
            Console.WriteLine(exception);
        }
    }

    public async Task LoadSettings()
    {
        var mainViewModel = Ioc.Default.GetService<MainViewModel>();
        if (mainViewModel is null)
        {
            return;
        }
        var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        var appSettingPath = Path.Combine(appDataPath, SettingsFolderName, SettingsFileName);
        if (File.Exists(appSettingPath))
        {
            await using var stream = File.OpenRead(appSettingPath);
            // TODO: Remove usage of stream param.
            await mainViewModel.LoadSettings(stream);
        }
    }

    public async Task SaveSettings()
    {
        var mainViewModel = Ioc.Default.GetService<MainViewModel>();
        if (mainViewModel is null)
        {
            return;
        }
        var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        var appPath = Path.Combine(appDataPath, SettingsFolderName);
        if (!Directory.Exists(appPath))
        {
            Directory.CreateDirectory(appPath);
        }
        var appSettingPath = Path.Combine(appPath, SettingsFileName);
        await using var stream = File.Open(appSettingPath, FileMode.Create);
        // TODO: Remove usage of stream param.
        await mainViewModel.SaveSettings(stream);
    }

    public void SaveTheme()
    {
        var theme = "Light";
        if (RequestedThemeVariant == ThemeVariant.Dark)
        {
            theme = "Dark";
        }

        var mainViewModel = Ioc.Default.GetService<MainViewModel>();
        if (mainViewModel is { })
        {
            mainViewModel.Theme = theme;
        }
    }

    public void ToggleAcrylicBlur()
    {
        if (Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            if (desktop.MainWindow is MainWindow mainWindow)
            {
                if (mainWindow.TransparencyLevelHint == WindowTransparencyLevel.AcrylicBlur)
                {
                    mainWindow.SystemDecorations = SystemDecorations.None;
                    mainWindow.ExtendClientAreaChromeHints = ExtendClientAreaChromeHints.NoChrome;
                    mainWindow.TransparencyLevelHint = WindowTransparencyLevel.Transparent;
                    mainWindow.AcrylicBorder.IsVisible = false;
                }
                else
                {
                    mainWindow.SystemDecorations = SystemDecorations.Full;
                    mainWindow.ExtendClientAreaChromeHints = ExtendClientAreaChromeHints.PreferSystemChrome;
                    mainWindow.TransparencyLevelHint = WindowTransparencyLevel.AcrylicBlur;
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
        var app = Ioc.Default.GetService<IApplicationService>();
        if (app is { })
        {
            app.Exit();
        }
    }
}
