using System.Text.Json.Serialization;
using ChatGPT.Model.Services;
using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Input;

namespace ChatGPT.ViewModels;

public partial class MainViewModel
{
    private string? _theme;
    private bool _showMenu;
    private bool _showSettings;
    private bool _showChats;
    private bool _showPrompts;

    [JsonPropertyName("theme")]
    public string? Theme
    {
        get => _theme;
        set => SetProperty(ref _theme, value);
    }

    [JsonIgnore]
    public bool ShowMenu
    {
        get => _showMenu;
        set => SetProperty(ref _showMenu, value);
    }

    [JsonIgnore]
    public bool ShowSettings
    {
        get => _showSettings;
        set => SetProperty(ref _showSettings, value);
    }

    [JsonIgnore]
    public bool ShowChats
    {
        get => _showChats;
        set => SetProperty(ref _showChats, value);
    }

    [JsonIgnore]
    public bool ShowPrompts
    {
        get => _showPrompts;
        set => SetProperty(ref _showPrompts, value);
    }

    [JsonIgnore]
    public IRelayCommand ExitCommand { get; }

    [JsonIgnore]
    public IRelayCommand ChangeThemeCommand { get; }

    [JsonIgnore]
    public IRelayCommand ShowSettingsCommand { get; }

    [JsonIgnore]
    public IRelayCommand ShowChatsCommand { get; }

    [JsonIgnore]
    public IRelayCommand ShowPromptsCommand { get; }

    private void ExitAction()
    {
        var app = Ioc.Default.GetService<IApplicationService>();
        app?.Exit();
    }

    private void ChangeThemeAction()
    {
        var app = Ioc.Default.GetService<IApplicationService>();
        if (app is { })
        {
            app.ToggleTheme();
        }
    }

    private void HideMenusAction()
    {
        ShowSettings = false;
        ShowChats = false;
        ShowPrompts = false;
        ShowMenu = false;
    }

    private void ShowSettingsAction()
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

    private void ShowChatsAction()
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

    private void ShowPromptsAction()
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
}
