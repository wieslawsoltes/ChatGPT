/*
 * SPDX-License-Identifier: AGPL-3.0-or-later
 * Copyright (C) 2022-2023, Wiesław Šoltés. All rights reserved.
 */
using ChatGPT.Model.Services;

namespace ChatGPT.Services;

public class IsolatedStorageFactory : IStorageFactory
{
    public IStorageService<T> CreateStorageService<T>() => new IsolatedStorageService<T>();
}
