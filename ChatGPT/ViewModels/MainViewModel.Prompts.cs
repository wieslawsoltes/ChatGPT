/*
 * SPDX-License-Identifier: AGPL-3.0-or-later
 * Copyright (C) 2022-2023, Wiesław Šoltés. All rights reserved.
 */
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using ChatGPT.Model.Services;
using ChatGPT.ViewModels.Settings;
using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Input;
using Microsoft.VisualBasic.FileIO;

namespace ChatGPT.ViewModels;

public partial class MainViewModel
{
    private ObservableCollection<PromptViewModel> _prompts;
    private PromptViewModel? _currentPrompt;

    [JsonPropertyName("prompts")]
    public ObservableCollection<PromptViewModel> Prompts
    {
        get => _prompts;
        set => SetProperty(ref _prompts, value);
    }

    [JsonPropertyName("currentPrompt")]
    public PromptViewModel? CurrentPrompt
    {
        get => _currentPrompt;
        set => SetProperty(ref _currentPrompt, value);
    }

    [JsonIgnore]
    public IAsyncRelayCommand AddPromptCommand { get; }

    [JsonIgnore]
    public IAsyncRelayCommand DeletePromptCommand { get; }

    [JsonIgnore]
    public IAsyncRelayCommand OpenPromptsCommand { get; }

    [JsonIgnore]
    public IAsyncRelayCommand SavePromptsCommand { get; }

    [JsonIgnore]
    public IAsyncRelayCommand ImportPromptsCommand { get; }

    [JsonIgnore]
    public IAsyncRelayCommand CopyPromptCommand { get; }

    [JsonIgnore]
    public IAsyncRelayCommand SetPromptCommand { get; }

    private async Task AddPromptAction()
    {
        NewPromptCallback();
        await Task.Yield();
    }

    private async Task DeletePromptAction()
    {
        DeletePromptCallback();
        await Task.Yield();
    }

    private async Task OpenPromptsAction()
    {
        var app = Ioc.Default.GetService<IApplicationService>();
        if (app is { })
        {
            await app.OpenFile(
                OpenPromptsCallbackAsync, 
                new List<string>(new[] { "Json", "All" }), 
                "Open");
        }
    }

    private async Task SavePromptsAction()
    {
        var app = Ioc.Default.GetService<IApplicationService>();
        if (app is { } && CurrentChat is { })
        {
            await app.SaveFile(
                SavePromptsCallbackAsync, 
                new List<string>(new[] { "Json", "All" }), 
                "Save", 
                "prompts", 
                "json");
        }
    }

    private async Task ImportPromptsAction()
    {
        var app = Ioc.Default.GetService<IApplicationService>();
        if (app is { })
        {
            await app.OpenFile(
                ImportPromptsCallbackAsync, 
                new List<string>(new[] { "Csv", "All" }), 
                "Import");
        }
    }

    private async Task CopyPromptAction()
    {
        var app = Ioc.Default.GetService<IApplicationService>();
        if (app is { } && CurrentPrompt?.Prompt is { })
        {
            await app.SetClipboardText(CurrentPrompt.Prompt);
        }
    }

    private async Task SetPromptAction()
    {
        SetPromptCallback();

        if (CurrentLayout is { })
        {
            await CurrentLayout.Back();
        }

        await Task.Yield();
    }

    private void NewPromptCallback()
    {
        var prompt = new PromptViewModel
        {
            Act = "Assistant",
            Prompt = Defaults.DefaultDirections
        };
        _prompts.Add(prompt);

        CurrentPrompt = prompt;
    }

    private void DeletePromptCallback()
    {
        if (CurrentPrompt is { })
        {
            Prompts.Remove(CurrentPrompt);
            CurrentPrompt = Prompts.LastOrDefault();
        }
    }

    private async Task OpenPromptsCallbackAsync(Stream stream)
    {
        var prompts = await JsonSerializer.DeserializeAsync(
            stream, 
            s_serializerContext.ObservableCollectionPromptViewModel);
        if (prompts is { })
        {
            foreach (var prompt in prompts)
            {
                Prompts.Add(prompt);
            }
        }
    }

    private async Task SavePromptsCallbackAsync(Stream stream)
    {
        await JsonSerializer.SerializeAsync(
            stream, 
            Prompts, s_serializerContext.ObservableCollectionPromptViewModel);
    }

    private async Task ImportPromptsCallbackAsync(Stream stream)
    {
        using var streamReader = new StreamReader(stream);
        var csv = await streamReader.ReadToEndAsync();
        using var stringReader = new StringReader(csv);
        using var parser = new TextFieldParser(stringReader);

        parser.HasFieldsEnclosedInQuotes = true;
        parser.Delimiters = new[] { "," };

        var haveHeader = false;

        while (!parser.EndOfData)
        {
            var fields = parser.ReadFields();
            if (fields is { } && fields.Length == 2)
            {
                if (haveHeader)
                {
                    var prompt = new PromptViewModel
                    {
                        Act = fields[0],
                        Prompt = fields[1]
                    };
                    Prompts.Add(prompt);
                }
                else
                {
                    // skip
                    haveHeader = true;
                }
            }
        }

        await Task.Yield();
    }

    private void SetPromptCallback()
    {
        if (CurrentPrompt is { } && CurrentChat?.Settings is { })
        {
            CurrentChat.Settings.Directions = CurrentPrompt.Prompt;
        }
    }
}
