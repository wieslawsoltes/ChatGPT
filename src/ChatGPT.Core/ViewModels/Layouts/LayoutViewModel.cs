using System.Text.Json.Serialization;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Input;

namespace ChatGPT.ViewModels.Layouts;

[JsonPolymorphic]
[JsonDerivedType(typeof(MobileLayoutViewModel), typeDiscriminator: "mobile")]
[JsonDerivedType(typeof(DesktopLayoutViewModel), typeDiscriminator: "desktop")]
public abstract partial class LayoutViewModel : ViewModelBase
{
    [JsonConstructor]
    protected LayoutViewModel()
    {
        ShowSettingsCommand = new RelayCommand(ShowSettingsAction);

        ShowChatsCommand = new RelayCommand(ShowChatsAction);

        ShowPromptsCommand = new RelayCommand(ShowPromptsAction);
    }

    [JsonPropertyName("name")]
    public partial string? Name { get; set; }

    [JsonPropertyName("showSettings")]
    public partial bool ShowSettings { get; set; }

    [JsonPropertyName("showChats")]
    public partial bool ShowChats { get; set; }

    [JsonPropertyName("showPrompts")]
    public partial bool ShowPrompts { get; set; }

    [JsonIgnore]
    public IRelayCommand ShowSettingsCommand { get; }

    [JsonIgnore]
    public IRelayCommand ShowChatsCommand { get; }

    [JsonIgnore]
    public IRelayCommand ShowPromptsCommand { get; }

    public abstract Task BackAsync();

    protected abstract void ShowSettingsAction();

    protected abstract void ShowChatsAction();

    protected abstract void ShowPromptsAction();

    public abstract LayoutViewModel Copy();
}
