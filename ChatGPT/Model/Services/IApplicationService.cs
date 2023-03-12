/*
 * SPDX-License-Identifier: AGPL-3.0-or-later
 * Copyright (C) 2022-2023, Wiesław Šoltés. All rights reserved.
 */
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace ChatGPT.Model.Services;

public interface IApplicationService
{
    Task OpenFile(Func<Stream, Task> callback, List<string> fileTypes, string title);
    Task SaveFile(Func<Stream, Task> callback, List<string> fileTypes, string title, string fileName, string defaultExtension);
    void ToggleTheme();
    Task SetClipboardText(string text);
    void Exit();
}
