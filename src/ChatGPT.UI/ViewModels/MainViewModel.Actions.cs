using System.Collections.ObjectModel;
using System.Text.Json.Serialization;
using ChatGPT.ViewModels.Layouts;
using CommunityToolkit.Mvvm.Input;

namespace ChatGPT.ViewModels;

public partial class MainViewModel
{
    [JsonPropertyName("layouts")]
    public partial ObservableCollection<LayoutViewModel>? Layouts { get; set; }

    [JsonPropertyName("currentLayout")]
    public partial LayoutViewModel? CurrentLayout { get; set; }

    [JsonPropertyName("mobileLayout")]
    public partial MobileLayoutViewModel? MobileLayout { get; set; }

    [JsonPropertyName("desktopLayout")]
    public partial DesktopLayoutViewModel? DesktopLayout { get; set; }

    [JsonPropertyName("theme")]
    public partial string? Theme { get; set; }

    [JsonPropertyName("topmost")]
    public partial bool Topmost { get; set; }

    [JsonIgnore]
    public IRelayCommand ExitCommand { get; }

    [JsonIgnore]
    public IAsyncRelayCommand LoadWorkspaceCommand { get; }

    [JsonIgnore]
    public IAsyncRelayCommand SaveWorkspaceCommand { get; }

    [JsonIgnore]
    public IAsyncRelayCommand ExportWorkspaceCommand { get; }

    [JsonIgnore]
    public IRelayCommand ChangeThemeCommand { get; }

    [JsonIgnore]
    public IRelayCommand ChangeDesktopMobileCommand { get; }

    [JsonIgnore]
    public IRelayCommand ChangeTopmostCommand { get; }

    private void ExitAction()
    {
        _applicationService?.Exit();
    }

    private void ChangeThemeAction()
    {
        if (_applicationService is { })
        {
            _applicationService.ToggleTheme();
        }
    }

    private void ChangeDesktopMobileAction()
    {
        CurrentLayout = CurrentLayout switch
        {
            MobileLayoutViewModel => DesktopLayout,
            DesktopLayoutViewModel => MobileLayout,
            _ => CurrentLayout
        };
    }

    private void ChangeTopmostAction()
    {
        Topmost = !Topmost;
    }
}
