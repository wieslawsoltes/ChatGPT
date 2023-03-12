/*
 * SPDX-License-Identifier: AGPL-3.0-or-later
 * Copyright (C) 2022-2023, Wiesław Šoltés. All rights reserved.
 */
namespace AI.Model.Services;

public class CompletionsServiceSettings
{
    public string? Model { get; set; }
    public string? Prompt { get; set; }
    public string? Suffix { get; set; }
    public decimal Temperature { get; set; }
    public int MaxTokens { get; set; }
    public decimal TopP { get; set; }
    public string? Stop { get; set; }
}
