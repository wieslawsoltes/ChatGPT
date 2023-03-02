using System.Collections.ObjectModel;

namespace ChatGPT.ViewModels;

public class StorageViewModel
{
    public ObservableCollection<ChatViewModel>? Chats { get; set; }

    public ChatViewModel? CurrentChat { get; set; }

    public string? Theme { get; set; }
}
