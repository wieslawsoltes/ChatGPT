using System.Collections.ObjectModel;
using System.Text.Json.Serialization;

namespace ChatGPT.ViewModels;

[JsonSerializable(typeof(MainViewModel))]
[JsonSerializable(typeof(MessageViewModel))]
[JsonSerializable(typeof(ChatViewModel))]
[JsonSerializable(typeof(ChatSettingsViewModel))]
[JsonSerializable(typeof(SettingsViewModel))]
[JsonSerializable(typeof(ActionsViewModel))]
[JsonSerializable(typeof(ObservableCollection<MessageViewModel>))]
[JsonSerializable(typeof(ObservableCollection<ChatViewModel>))]
public partial class MainViewModelJsonContext : JsonSerializerContext
{
}
