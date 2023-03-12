/*
 * SPDX-License-Identifier: AGPL-3.0-or-later
 * Copyright (C) 2022-2023, Wiesław Šoltés. All rights reserved.
 */
using System.Collections.ObjectModel;
using ChatGPT.ViewModels.Chat;

namespace ChatGPT.Model.Plugins;

public interface IPluginContext
{
    ObservableCollection<ChatViewModel> Chats { get; set; }
    ChatViewModel? CurrentChat { get; set; }
}
