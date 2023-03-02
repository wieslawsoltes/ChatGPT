using System.Collections.ObjectModel;
using System.Text.Json.Serialization;

namespace ChatGPT.ViewModels;

[JsonSerializable(typeof(ChatMessageViewModel))]
[JsonSerializable(typeof(ChatViewModel))]
[JsonSerializable(typeof(ObservableCollection<ChatViewModel>))]
[JsonSerializable(typeof(ChatSettingsViewModel))]
[JsonSerializable(typeof(ObservableCollection<ChatMessageViewModel>))]
[JsonSerializable(typeof(ActionsViewModel))]
[JsonSerializable(typeof(SettingsViewModel))]
[JsonSerializable(typeof(MainViewModel))]
public partial class MainViewModelJsonContext : JsonSerializerContext
{
}
