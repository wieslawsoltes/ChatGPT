using System.Collections.ObjectModel;
using System.Text.Json.Serialization;
using ChatGPT.Model.Services;
using ChatGPT.ViewModels.Layouts;
using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Input;

namespace ChatGPT.ViewModels;

public partial class MainViewModel
{
    private ObservableCollection<LayoutViewModel>? _layouts;
    private LayoutViewModel? _currentLayout;
    private string? _theme;
    private string? _layout;

    [JsonIgnore]
    public ObservableCollection<LayoutViewModel>? Layouts
    {
        get => _layouts;
        set => SetProperty(ref _layouts, value);
    }

    [JsonIgnore]
    public LayoutViewModel? CurrentLayout
    {
        get => _currentLayout;
        set => SetProperty(ref _currentLayout, value);
    }

    public SingleLayoutViewModel? SingleLayout { get; private set; }

    public ColumnLayoutViewModel? ColumnLayout { get; private set; }

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

    [JsonIgnore]
    public IRelayCommand ExitCommand { get; }

    [JsonIgnore]
    public IRelayCommand ChangeThemeCommand { get; }

    [JsonIgnore]
    public IRelayCommand ChangeDesktopMobileCommand { get; }

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
}
