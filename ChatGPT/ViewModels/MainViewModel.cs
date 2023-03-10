using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using ChatGPT.Model.Plugins;
using ChatGPT.ViewModels.Chat;
using ChatGPT.ViewModels.Layouts;
using ChatGPT.ViewModels.Settings;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace ChatGPT.ViewModels;

public partial class MainViewModel : ObservableObject, IPluginContext
{
    private static readonly MainViewModelJsonContext s_serializerContext = new(
        new JsonSerializerOptions
        {
            WriteIndented = true,
            ReferenceHandler = ReferenceHandler.Preserve,
            IncludeFields = false,
            IgnoreReadOnlyFields = true,
            IgnoreReadOnlyProperties = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            NumberHandling = JsonNumberHandling.AllowNamedFloatingPointLiterals,
        });

    private double _width;
    private double _height;

    public MainViewModel()
    {
        _chats = new ObservableCollection<ChatViewModel>();
        _prompts = new ObservableCollection<PromptViewModel>();

        SingleLayout = new SingleLayoutViewModel();

        ColumnLayout = new ColumnLayoutViewModel();

        _layouts = new ObservableCollection<LayoutViewModel>
        {
            SingleLayout,
            ColumnLayout
        };

        CurrentLayout = SingleLayout;

        Layout = "Mobile";

        Width = 400;
        Height = 740;
        
        NewPromptCallback();

        NewChatCallback();

        // Chats
        
        AddChatCommand = new AsyncRelayCommand(AddChatAction);

        DeleteChatCommand = new AsyncRelayCommand(DeleteChatAction);

        OpenChatCommand = new AsyncRelayCommand(OpenChatAction);

        SaveChatCommand = new AsyncRelayCommand(SaveChatAction);

        ExportChatCommand = new AsyncRelayCommand(ExportChatAction);

        CopyChatCommand = new AsyncRelayCommand(CopyChatAction);

        DefaultChatSettingsCommand = new RelayCommand(DefaultChatSettingsAction);

        // Prompts

        AddPromptCommand = new AsyncRelayCommand(AddPromptAction);

        DeletePromptCommand = new AsyncRelayCommand(DeletePromptAction);

        OpenPromptsCommand = new AsyncRelayCommand(OpenPromptsAction);

        SavePromptsCommand = new AsyncRelayCommand(SavePromptsAction);

        ImportPromptsCommand = new AsyncRelayCommand(ImportPromptsAction);

        CopyPromptCommand = new AsyncRelayCommand(CopyPromptAction);

        SetPromptCommand = new AsyncRelayCommand(SetPromptAction);

        // Actions
    
        ExitCommand = new RelayCommand(ExitAction);

        ChangeThemeCommand = new RelayCommand(ChangeThemeAction);

        ChangeDesktopMobileCommand = new RelayCommand(ChangeDesktopMobileAction);
    }

    [JsonPropertyName("width")]
    public double Width
    {
        get => _width;
        set => SetProperty(ref _width, value);
    }

    [JsonPropertyName("height")]
    public double Height
    {
        get => _height;
        set => SetProperty(ref _height, value);
    }

    public async Task LoadSettings(Stream stream)
    {
        var storage = await JsonSerializer.DeserializeAsync(
            stream, 
            s_serializerContext.StorageViewModel);
        if (storage is { })
        {
            if (storage.Chats is { })
            {
                foreach (var chat in storage.Chats)
                {
                    foreach (var message in chat.Messages)
                    {
                        chat.SetMessageActions(message);
                    }
                }

                Chats = storage.Chats;
                CurrentChat = storage.CurrentChat;
            }

            if (storage.Prompts is { })
            {
                Prompts = storage.Prompts;
                CurrentPrompt = storage.CurrentPrompt;
            }

            if (storage.Layouts is { })
            {
                Layouts = storage.Layouts;
                CurrentLayout = storage.CurrentLayout;
                SingleLayout = Layouts.OfType<SingleLayoutViewModel>().FirstOrDefault();
                ColumnLayout = Layouts.OfType<ColumnLayoutViewModel>().FirstOrDefault();
            }

            if (storage.Layout is { })
            {
                Layout = storage.Layout;
            }

            if (storage.Theme is { })
            {
                Theme = storage.Theme;
            }

            if (storage.Width is { })
            {
                Width = storage.Width.Value;
            }

            if (storage.Height is { })
            {
                Height = storage.Height.Value;
            }
        }
    }

    public async Task SaveSettings(Stream stream)
    {
        var storage = new StorageViewModel
        {
            Chats = Chats,
            CurrentChat = CurrentChat,
            Prompts = Prompts,
            CurrentPrompt = CurrentPrompt,
            Layouts = Layouts,
            CurrentLayout = CurrentLayout,
            Theme = Theme,
            Layout = Layout,
            Width = Width,
            Height = Height
        };
        await JsonSerializer.SerializeAsync(
            stream, 
            storage, s_serializerContext.StorageViewModel);
    }
}
