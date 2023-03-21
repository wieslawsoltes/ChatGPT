using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ChatGPT.ViewModels.Layouts;

public partial class ColumnLayoutViewModel : LayoutViewModel
{
    private string _settingsWidth;
    private string _chatsWidth;
    private string _promptsWidth;

    [JsonConstructor]
    public ColumnLayoutViewModel()
    {
        ShowChats = true;
        ShowSettings = true;
        ShowPrompts = true;

        _settingsWidth = "250";
        _chatsWidth = "290";
        _promptsWidth = "290";
    }

    [JsonPropertyName("settingsWidth")]
    public string SettingsWidth
    {
        get => _settingsWidth;
        set => SetProperty(ref _settingsWidth, value);
    }

    [JsonPropertyName("chatsWidth")]
    public string ChatsWidth
    {
        get => _chatsWidth;
        set => SetProperty(ref _chatsWidth, value);
    }

    [JsonPropertyName("promptsWidth")]
    public string PromptsWidth
    {
        get => _promptsWidth;
        set => SetProperty(ref _promptsWidth, value);
    }

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
}
