using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ChatGPT.ViewModels.Layouts;

public partial class DesktopLayoutViewModel : LayoutViewModel
{
    [JsonConstructor]
    public DesktopLayoutViewModel()
    {
        Name = "Desktop";

        ShowChats = true;
        ShowSettings = true;
        ShowPrompts = true;

        _settingsWidth = "290";
        _chatsWidth = "290";
        _promptsWidth = "290";
    }

    [JsonPropertyName("settingsWidth")]
    public partial string SettingsWidth { get; set; }

    [JsonPropertyName("chatsWidth")]
    public partial string ChatsWidth { get; set; }

    [JsonPropertyName("promptsWidth")]
    public partial string PromptsWidth { get; set; }

    public override async Task BackAsync()
    {
        // TODO:
        await Task.Yield();
    }

    protected override void ShowSettingsAction()
    {
        ShowSettings = !ShowSettings;
    }

    protected override void ShowChatsAction()
    {
        ShowChats = !ShowChats;
    }

    protected override void ShowPromptsAction()
    {
        ShowPrompts = !ShowPrompts;
    }

    public override LayoutViewModel Copy()
    {
        return new DesktopLayoutViewModel()
        {
            ShowChats = ShowChats,
            ShowSettings = ShowSettings,
            ShowPrompts = ShowPrompts,
            SettingsWidth = _settingsWidth,
            ChatsWidth = _chatsWidth,
            PromptsWidth = _promptsWidth
        };
    }
}
