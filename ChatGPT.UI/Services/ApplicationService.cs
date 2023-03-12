/*
 * SPDX-License-Identifier: AGPL-3.0-or-later
 * Copyright (C) 2022-2023, Wiesław Šoltés. All rights reserved.
 */
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Platform.Storage;
using Avalonia.Styling;
using Avalonia.VisualTree;
using ChatGPT.Model.Services;

namespace ChatGPT.Services;

public class ApplicationService : IApplicationService
{
    private static FilePickerFileType All { get; } = new("All")
    {
        Patterns = new[] { "*.*" },
        MimeTypes = new[] { "*/*" }
    };

    private static FilePickerFileType Json { get; } = new("Json")
    {
        Patterns = new[] { "*.json" },
        AppleUniformTypeIdentifiers = new[] { "public.json" },
        MimeTypes = new[] { "application/json" }
    };

    private static FilePickerFileType Text { get; } = new("Text")
    {
        Patterns = new[] { "*.txt" },
        AppleUniformTypeIdentifiers = new[] { "public.text" },
        MimeTypes = new[] { "text/plain" }
    };

    private static IStorageProvider? GetStorageProvider()
    {
        if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime { MainWindow: { } window })
        {
            return window.StorageProvider;
        }

        if (Application.Current?.ApplicationLifetime is ISingleViewApplicationLifetime { MainView: { } mainView })
        {
            var visualRoot = mainView.GetVisualRoot();
            if (visualRoot is TopLevel topLevel)
            {
                return topLevel.StorageProvider;
            }
        }

        return null;
    }

    private static List<FilePickerFileType> GetFilePickerFileTypes(List<string> fileTypes)
    {
        var fileTypeFilters = new List<FilePickerFileType>();

        foreach (var fileType in fileTypes)
        {
            switch (fileType)
            {
                case "All":
                {
                    fileTypeFilters.Add(All);
                    break;
                }
                case "Json":
                {
                    fileTypeFilters.Add(Json);
                    break;
                }
                case "Text":
                {
                    fileTypeFilters.Add(Text);
                    break;
                }
            }
        }

        return fileTypeFilters;
    }

    public async Task OpenFile(Func<Stream, Task> callback, List<string> fileTypes, string title)
    {
        var storageProvider = GetStorageProvider();
        if (storageProvider is null)
        {
            return;
        }

        var result = await storageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = title,
            FileTypeFilter = GetFilePickerFileTypes(fileTypes),
            AllowMultiple = false
        });

        var file = result.FirstOrDefault();
        if (file is not null)
        {
            try
            {
                await using var stream = await file.OpenReadAsync();
                await callback(stream);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }

    public async Task SaveFile(Func<Stream, Task> callback, List<string> fileTypes, string title, string fileName, string defaultExtension)
    {
        var storageProvider = GetStorageProvider();
        if (storageProvider is null)
        {
            return;
        }

        var file = await storageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
        {
            Title = title,
            FileTypeChoices = GetFilePickerFileTypes(fileTypes),
            SuggestedFileName = fileName,
            DefaultExtension = defaultExtension,
            ShowOverwritePrompt = true
        });

        if (file is not null)
        {
            try
            {
                await using var stream = await file.OpenWriteAsync();
                await callback(stream);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }

    public void ToggleTheme()
    {
        if (Application.Current is { })
        {
            Application.Current.RequestedThemeVariant = 
                Application.Current.RequestedThemeVariant == ThemeVariant.Light 
                    ? ThemeVariant.Dark 
                    : ThemeVariant.Light;
        }
    }

    public async Task SetClipboardText(string text)
    {
        try
        {
            if (Application.Current?.Clipboard is { } clipboard)
            {
                await clipboard.SetTextAsync(text);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }

    public void Exit()
    {
        if (Application.Current?.ApplicationLifetime is IControlledApplicationLifetime lifetime)
        {
            lifetime.Shutdown();
        }
    }
}
