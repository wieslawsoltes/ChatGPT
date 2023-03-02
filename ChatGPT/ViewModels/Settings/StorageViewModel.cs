using System.Collections.ObjectModel;
using ChatGPT.ViewModels.Chat;

namespace ChatGPT.ViewModels.Settings;

public class StorageViewModel
{
    public ObservableCollection<ChatViewModel>? Chats { get; set; }

    public ChatViewModel? CurrentChat { get; set; }

    public string? Theme { get; set; }
}
