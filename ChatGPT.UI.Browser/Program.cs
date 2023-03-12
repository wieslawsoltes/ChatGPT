/*
 * SPDX-License-Identifier: AGPL-3.0-or-later
 * Copyright (C) 2022-2023, Wiesław Šoltés. All rights reserved.
 */
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
                Ioc.Default.ConfigureServices(
                    new ServiceCollection()
                        // Services
                        .AddSingleton<IStorageFactory, BrowserStorageFactory>()
                        .AddSingleton<IApplicationService, ApplicationService>()
                        .AddSingleton<IPluginsService, PluginsService>()
                        .AddSingleton<IChatService, ChatService>()
                        .AddSingleton<ICompletionsService, CompletionsService>()
                        .AddSingleton<MainViewModel>()
                        .AddSingleton<IPluginContext>(x => x.GetRequiredService<MainViewModel>())
                        // ViewModels
                        .AddTransient<ChatMessageViewModel>()
                        .AddTransient<ChatSettingsViewModel>()
                        .AddTransient<ChatViewModel>()
                        .AddTransient<PromptViewModel>()
                        .AddTransient<WorkspaceViewModel>()
                        .BuildServiceProvider());
            });
}
