/*
 * SPDX-License-Identifier: AGPL-3.0-or-later
 * Copyright (C) 2022-2023, Wiesław Šoltés. All rights reserved.
 */
using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices.JavaScript;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;
using System.Threading.Tasks;
using ChatGPT.Model.Services;

namespace ChatGPT.UI.Browser.Services;

public partial class BrowserStorageService<T> : IStorageService<T>
{
    [JSImport("globalThis.localStorage.setItem")]
    private static partial void SetItem(string key, string value);

    [JSImport("globalThis.localStorage.getItem")]
    private static partial string GetItem(string key);

    private static string Identifier { get; } = typeof(T).FullName?.Replace(".", string.Empty) ?? "default";

    public async Task SaveObject(T obj, string key, JsonTypeInfo<T> typeInfo)
    {
        var stream = new MemoryStream();
        await JsonSerializer.SerializeAsync(stream, obj, typeInfo);
        stream.Position = 0;
        var serializedObjJson = Encoding.Unicode.GetString(stream.ToArray());
        SetItem(Identifier + key, serializedObjJson);
    }

    public async Task<T?> LoadObject(string key, JsonTypeInfo<T> typeInfo)
    {
        try
        {
            await Task.Delay(1);
            var t = GetItem(Identifier + key);
            if (string.IsNullOrEmpty(t)) return default;
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(t));
            var x = await JsonSerializer.DeserializeAsync(stream, typeInfo);
            return x ?? default;
        }
        catch (Exception e)
        {
            Debug.WriteLine(e);
        }

        return default;
    }
}
