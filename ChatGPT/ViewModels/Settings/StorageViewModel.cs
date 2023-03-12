/*
 * SPDX-License-Identifier: AGPL-3.0-or-later
 * Copyright (C) 2022-2023, Wiesław Šoltés. All rights reserved.
 */
using System.Collections.ObjectModel;
using ChatGPT.ViewModels.Chat;
using ChatGPT.ViewModels.Layouts;

namespace ChatGPT.ViewModels.Settings;

public class StorageViewModel
{
    public ObservableCollection<ChatViewModel>? Chats { get; set; }

    public ChatViewModel? CurrentChat { get; set; }

    public ObservableCollection<PromptViewModel>? Prompts { get; set; }

    public PromptViewModel? CurrentPrompt { get; set; }

    public ObservableCollection<LayoutViewModel>? Layouts { get; set; }

    public LayoutViewModel? CurrentLayout { get; set; }
    
    public string? Theme { get; set; }

    public string? Layout { get; set; }

    public double? Width { get; set; }

    public double? Height { get; set; }
}
