/*
 * SPDX-License-Identifier: AGPL-3.0-or-later
 * Copyright (C) 2022-2023, Wiesław Šoltés. All rights reserved.
 */
using System.Text.Json.Serialization;

namespace AI.Model.Json.Completions;

[JsonSerializable(typeof(CompletionsRequestBody))]
[JsonSerializable(typeof(CompletionsResponseSuccess))]
[JsonSerializable(typeof(CompletionsChoice))]
[JsonSerializable(typeof(CompletionsUsage))]
[JsonSerializable(typeof(CompletionsResponseError))]
[JsonSerializable(typeof(CompletionsError))]
public partial class CompletionsJsonContext : JsonSerializerContext
{
}
