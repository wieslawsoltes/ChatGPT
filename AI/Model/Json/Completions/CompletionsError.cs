/*
 * SPDX-License-Identifier: AGPL-3.0-or-later
 * Copyright (C) 2022-2023, Wiesław Šoltés. All rights reserved.
 */
using System.Text.Json.Serialization;

namespace AI.Model.Json.Completions;

public class CompletionsError
{
    [JsonPropertyName("message")]
    public string? Message { get; set; }
    [JsonPropertyName("type")]
    public string? Type { get; set; }
    [JsonPropertyName("param")]
    public object? Param { get; set; }
    [JsonPropertyName("code")]
    public string? Code { get; set; }
}
