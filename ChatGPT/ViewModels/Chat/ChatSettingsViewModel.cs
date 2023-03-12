/*
 * SPDX-License-Identifier: AGPL-3.0-or-later
 * Copyright (C) 2022-2023, Wiesław Šoltés. All rights reserved.
 */
using System.Text.Json.Serialization;
using CommunityToolkit.Mvvm.ComponentModel;

namespace ChatGPT.ViewModels.Chat;

public partial class ChatSettingsViewModel : ObservableObject
{
    private decimal _temperature;
    private int _maxTokens;
    private string? _apiKey;
    private string? _model;
    private string? _directions;
    private string? _format;

    [JsonPropertyName("temperature")]
    public decimal Temperature
    {
        get => _temperature;
        set => SetProperty(ref _temperature, value);
    }

    [JsonPropertyName("maxTokens")]
    public int MaxTokens
    {
        get => _maxTokens;
        set => SetProperty(ref _maxTokens, value);
    }

    [JsonPropertyName("apiKey")]
    public string? ApiKey
    {
        get => _apiKey;
        set => SetProperty(ref _apiKey, value);
    }

    [JsonPropertyName("model")]
    public string? Model
    {
        get => _model;
        set => SetProperty(ref _model, value);
    }

    [JsonPropertyName("directions")]
    public string? Directions
    {
        get => _directions;
        set => SetProperty(ref _directions, value);
    }

    [JsonPropertyName("format")]
    public string? Format
    {
        get => _format;
        set => SetProperty(ref _format, value);
    }

    public ChatSettingsViewModel Copy()
    {
        return new ChatSettingsViewModel
        {
            Temperature = _temperature,
            MaxTokens = _maxTokens,
            ApiKey = _apiKey,
            Model = _model,
            Directions = _directions,
            Format = _format,
        };
    }
}
