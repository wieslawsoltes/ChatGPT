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
}
