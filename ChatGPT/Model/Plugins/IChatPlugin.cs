/*
 * SPDX-License-Identifier: AGPL-3.0-or-later
 * Copyright (C) 2022-2023, Wiesław Šoltés. All rights reserved.
 */
using System.Threading.Tasks;

namespace ChatGPT.Model.Plugins;

public interface IChatPlugin
{
    string Id { get; }
    string Name { get; }
    Task Start();
    Task Stop();
    Task Initialize(IPluginContext context);
    Task Shutdown();
}
