/*
 * SPDX-License-Identifier: AGPL-3.0-or-later
 * Copyright (C) 2022-2023, Wiesław Šoltés. All rights reserved.
 */
using System.Threading.Tasks;
using AI.Model.Json.Completions;

namespace AI.Model.Services;

public interface ICompletionsService
{
    Task<CompletionsResponse?> GetResponseDataAsync(CompletionsServiceSettings settings);
}
