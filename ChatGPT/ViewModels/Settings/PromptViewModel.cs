/*
 * SPDX-License-Identifier: AGPL-3.0-or-later
 * Copyright (C) 2022-2023, Wiesław Šoltés. All rights reserved.
 */
using System.Text.Json.Serialization;
using CommunityToolkit.Mvvm.ComponentModel;

namespace ChatGPT.ViewModels.Settings;

public class PromptViewModel : ObservableObject
{
    private string? _act;
    private string? _prompt;

    [JsonPropertyName("act")]
    public string? Act
    {
        get => _act;
        set => SetProperty(ref _act, value);
    }

    [JsonPropertyName("prompt")]
    public string? Prompt
    {
        get => _prompt;
        set => SetProperty(ref _prompt, value);
    }
}
