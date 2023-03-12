/*
 * SPDX-License-Identifier: AGPL-3.0-or-later
 * Copyright (C) 2022-2023, Wiesław Šoltés. All rights reserved.
 */
using System.Text.Json.Serialization;

namespace AI.Model.Json.Completions;

public class CompletionsResponseSuccess : CompletionsResponse
{
    [JsonPropertyName("id")]
    public string? Id { get; set; }

    [JsonPropertyName("object")]
    public string? Object { get; set; } // Escaped with @ symbol

    [JsonPropertyName("created")]
    public int Created { get; set; }

    [JsonPropertyName("model")]
    public string? Model { get; set; }

    [JsonPropertyName("choices")]
    public CompletionsChoice[]? Choices { get; set; }

    [JsonPropertyName("usage")]
    public CompletionsUsage? Usage { get; set; }
}
