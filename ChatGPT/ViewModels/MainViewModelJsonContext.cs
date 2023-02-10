using System.Collections.ObjectModel;
using System.Text.Json.Serialization;

namespace ChatGPT.ViewModels;

[JsonSerializable(typeof(MainViewModel))]
[JsonSerializable(typeof(MessageViewModel))]
[JsonSerializable(typeof(SettingsViewModel))]
[JsonSerializable(typeof(ChatSettingsViewModel))]
[JsonSerializable(typeof(ActionsViewModel))]
[JsonSerializable(typeof(ObservableCollection<MessageViewModel>))]
public partial class MainViewModelJsonContext : JsonSerializerContext
{
}
