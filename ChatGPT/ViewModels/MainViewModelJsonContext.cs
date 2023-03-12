/*
 * SPDX-License-Identifier: AGPL-3.0-or-later
 * Copyright (C) 2022-2023, Wiesław Šoltés. All rights reserved.
 */
using System.Collections.ObjectModel;
using System.Text.Json.Serialization;
using ChatGPT.ViewModels.Chat;
using ChatGPT.ViewModels.Layouts;
using ChatGPT.ViewModels.Settings;

namespace ChatGPT.ViewModels;

[JsonSerializable(typeof(ChatMessageViewModel))]
[JsonSerializable(typeof(ChatViewModel))]
[JsonSerializable(typeof(ObservableCollection<ChatViewModel>))]
[JsonSerializable(typeof(ChatSettingsViewModel))]
[JsonSerializable(typeof(ObservableCollection<ChatMessageViewModel>))]
[JsonSerializable(typeof(MainViewModel))]
[JsonSerializable(typeof(PromptViewModel))]
[JsonSerializable(typeof(ObservableCollection<PromptViewModel>))]
[JsonSerializable(typeof(LayoutViewModel))]
[JsonSerializable(typeof(SingleLayoutViewModel))]
[JsonSerializable(typeof(ColumnLayoutViewModel))]
[JsonSerializable(typeof(ObservableCollection<LayoutViewModel>))]
[JsonSerializable(typeof(StorageViewModel))]
public partial class MainViewModelJsonContext : JsonSerializerContext
{
}
