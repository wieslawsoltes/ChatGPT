/*
 * SPDX-License-Identifier: AGPL-3.0-or-later
 * Copyright (C) 2022-2023, Wiesław Šoltés. All rights reserved.
 */
namespace ChatGPT.Model.Services;

public interface IStorageFactory
{
    IStorageService<T> CreateStorageService<T>();
}
