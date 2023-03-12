/*
 * SPDX-License-Identifier: AGPL-3.0-or-later
 * Copyright (C) 2022-2023, Wiesław Šoltés. All rights reserved.
 */
using ChatGPT.Model.Services;

namespace ChatGPT.UI.Browser.Services;

public class BrowserStorageFactory : IStorageFactory
{
    public IStorageService<T> CreateStorageService<T>() => new BrowserStorageService<T>();
}
