/*
 * SPDX-License-Identifier: AGPL-3.0-or-later
 * Copyright (C) 2022-2023, Wiesław Šoltés. All rights reserved.
 */
using System.Text.Json.Serialization;

namespace AI.Model.Json.Chat;

public class ChatResponseError : ChatResponse
{
    [JsonPropertyName("error")]
    public ChatError? Error { get; set; }
}
