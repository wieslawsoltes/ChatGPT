using System.Collections.ObjectModel;
using System.Text.Json.Serialization;
using ChatGPT.ViewModels.Chat;
using ChatGPT.ViewModels.Layouts;

namespace ChatGPT.ViewModels.Settings;

public partial class WorkspaceViewModel : ViewModelBase
{
    [JsonPropertyName("name")]
    public partial string? Name { get; set; }

    [JsonPropertyName("chats")]
    public partial ObservableCollection<ChatViewModel>?  Chats { get; set; }

    [JsonPropertyName("currentChat")]
    public partial ChatViewModel? CurrentChat { get; set; }
    
    [JsonPropertyName("prompts")]
    public partial ObservableCollection<PromptViewModel>? Prompts { get; set; }

    [JsonPropertyName("currentPrompt")]
    public partial PromptViewModel? CurrentPrompt { get; set; }

    [JsonPropertyName("layouts")]
    public partial ObservableCollection<LayoutViewModel>? Layouts { get; set; }

    [JsonPropertyName("currentLayout")]
    public partial LayoutViewModel? CurrentLayout { get; set; }

    [JsonPropertyName("theme")]
    public partial string? Theme { get; set; }

    [JsonPropertyName("topmost")]
    public partial bool Topmost { get; set; }

    private ObservableCollection<ChatViewModel>? CopyChats(out ChatViewModel? currentChat)
    {
        currentChat = null;

        if (_chats is null)
        {
            return null;
        }

        var chats = new ObservableCollection<ChatViewModel>();

        foreach (var chat in _chats)
        {
            var chatCopy = chat.Copy();

            chats.Add(chatCopy);

            if (chat == _currentChat)
            {
                currentChat = chatCopy;
            }
        }

        return chats;
    }

    private ObservableCollection<PromptViewModel>? CopyPrompts(out PromptViewModel? currentPrompt)
    {
        currentPrompt = null;

        if (_prompts is null)
        {
            return null;
        }

        var prompts = new ObservableCollection<PromptViewModel>();

        foreach (var prompt in _prompts)
        {
            var promptCopy = prompt.Copy();

            prompts.Add(promptCopy);

            if (prompt == _currentPrompt)
            {
                currentPrompt = promptCopy;
            }
        }

        return prompts;
    }

    private ObservableCollection<LayoutViewModel>? CopyLayouts(out LayoutViewModel? currentLayout)
    {
        currentLayout = null;

        if (_layouts is null)
        {
            return null;
        }

        var layouts = new ObservableCollection<LayoutViewModel>();

        foreach (var layout in _layouts)
        {
            var layoutCopy = layout.Copy();

            layouts.Add(layoutCopy);

            if (layout == _currentLayout)
            {
                currentLayout = layoutCopy;
            }
        }

        return layouts;
    }

    public WorkspaceViewModel Copy()
    {
        return new WorkspaceViewModel
        {
            Name = _name,
            Chats = CopyChats(out var currentChat),
            CurrentChat = currentChat,
            Prompts = CopyPrompts(out var currentPrompt),
            CurrentPrompt = currentPrompt,
            Layouts = CopyLayouts(out var currentLayout),
            CurrentLayout = currentLayout,
            Theme = _theme,
            Topmost = _topmost,
        };
    }
}
