/*
 * SPDX-License-Identifier: AGPL-3.0-or-later
 * Copyright (C) 2022-2023, Wiesław Šoltés. All rights reserved.
 */
using AI.Model.Json.Chat;

namespace AI.Model.Services;

public class ChatServiceSettings
{
    public string? Model { get; set; }
    public ChatMessage[]? Messages { get; set; }
    public string? Suffix { get; set; }
    public decimal Temperature { get; set; }
    public int MaxTokens { get; set; }
    public decimal TopP { get; set; }
    public string? Stop { get; set; }
}
