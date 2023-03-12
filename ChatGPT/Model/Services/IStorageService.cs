/*
 * SPDX-License-Identifier: AGPL-3.0-or-later
 * Copyright (C) 2022-2023, Wiesław Šoltés. All rights reserved.
 */
using System.Text.Json.Serialization.Metadata;
using System.Threading.Tasks;

namespace ChatGPT.Model.Services;

public interface IStorageService<T>
{
    Task SaveObject(T obj, string key, JsonTypeInfo<T> typeInfo);
    Task<T?> LoadObject(string key, JsonTypeInfo<T> typeInfo);
}
