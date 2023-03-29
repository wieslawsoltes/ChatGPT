using System.Collections.ObjectModel;
using System.Text.Json.Serialization;
using ChatGPT.Model.Services;
using ChatGPT.ViewModels.Layouts;
using CommunityToolkit.Mvvm.Input;

namespace ChatGPT.ViewModels;

public partial class MainViewModel
{
    private ObservableCollection<LayoutViewModel>? _layouts;
    private LayoutViewModel? _currentLayout;
    private string? _theme;
    private string? _layout;
    private bool _topmost;

    [JsonPropertyName("layouts")]
    public ObservableCollection<LayoutViewModel>? Layouts
    {
        get => _layouts;
        set => SetProperty(ref _layouts, value);
    }

    [JsonPropertyName("currentLayout")]
    public LayoutViewModel? CurrentLayout
    {
        get => _currentLayout;
        set => SetProperty(ref _currentLayout, value);
    }

    [JsonPropertyName("theme")]
    public string? Theme
    {
        get => _theme;
        set => SetProperty(ref _theme, value);
    }

    [JsonPropertyName("layout")]
    public string? Layout
    {
        get => _layout;
        set => SetProperty(ref _layout, value);
    }

    [JsonPropertyName("topmost")]
    public bool Topmost
    {
        get => _topmost;
        set => SetProperty(ref _topmost, value);
    }

    [JsonIgnore]
    public SingleLayoutViewModel? SingleLayout { get; private set; }

    [JsonIgnore]
    public ColumnLayoutViewModel? ColumnLayout { get; private set; }

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
        var app = Defaults.Locator.GetService<IApplicationService>();
        app?.Exit();
    }

    private void ChangeThemeAction()
    {
        var app = Defaults.Locator.GetService<IApplicationService>();
        if (app is { })
        {
            app.ToggleTheme();
        }
    }

    private void ChangeDesktopMobileAction()
    {
        switch (Layout)
        {
            case "Mobile":
                CurrentLayout = ColumnLayout;
                Layout = "Desktop";
                break;
            case "Desktop":
                CurrentLayout = SingleLayout;
                Layout = "Mobile";
                break;
        }
    }

    private void ChangeTopmostAction()
    {
        var app = Defaults.Locator.GetService<IApplicationService>();
        if (app is { })
        {
            Topmost = !Topmost;
        }
    }
}
