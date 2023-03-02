using System.Collections.ObjectModel;
using System.Text.Json.Serialization;
using ChatGPT.ViewModels.Chat;
using ChatGPT.ViewModels.Settings;

namespace ChatGPT.ViewModels;

[JsonSerializable(typeof(ChatMessageViewModel))]
[JsonSerializable(typeof(ChatViewModel))]
[JsonSerializable(typeof(ObservableCollection<ChatViewModel>))]
[JsonSerializable(typeof(ChatSettingsViewModel))]
[JsonSerializable(typeof(ObservableCollection<ChatMessageViewModel>))]
[JsonSerializable(typeof(MainViewModel))]
[JsonSerializable(typeof(StorageViewModel))]
public partial class MainViewModelJsonContext : JsonSerializerContext
{
}
