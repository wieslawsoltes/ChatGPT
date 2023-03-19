using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ChatGPT.ViewModels.Layouts;

public partial class SingleLayoutViewModel : LayoutViewModel
{
    private bool _showMenu;

    [JsonConstructor]
    public SingleLayoutViewModel()
    {
        ShowChats = false;
        ShowSettings = false;
        ShowPrompts = false;
        ShowMenu = false;
    }

    [JsonPropertyName("showMenu")]
    public bool ShowMenu
    {
        get => _showMenu;
        set => SetProperty(ref _showMenu, value);
    }

    public override async Task Back()
    {
        HideMenusAction();
        await Task.Yield();
    }

    protected override void ShowSettingsAction()
    {
        if (ShowMenu)
        {
            HideMenusAction();
        }
        else
        {
            ShowSettings = true;
            ShowChats = false;
            ShowPrompts = false;
            ShowMenu = true;
        }
    }

    protected override void ShowChatsAction()
    {
        if (ShowMenu)
        {
            HideMenusAction();
        }
        else
        {
            ShowChats = true;
            ShowSettings = false;
            ShowPrompts = false;
            ShowMenu = true;
        }
    }

    protected override void ShowPromptsAction()
    {
        if (ShowMenu)
        {
            HideMenusAction();
        }
        else
        {
            ShowPrompts = true;
            ShowChats = false;
            ShowSettings = false;
            ShowMenu = true;
        }
    }

    private void HideMenusAction()
    {
        ShowSettings = false;
        ShowChats = false;
        ShowPrompts = false;
        ShowMenu = false;
    }
}
