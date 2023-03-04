using System.Collections.ObjectModel;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using ChatGPT.Model.Plugins;
using ChatGPT.ViewModels.Chat;
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

    public MainViewModel()
    {
        _chats = new ObservableCollection<ChatViewModel>();
        _prompts = new ObservableCollection<PromptViewModel>();

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

        ShowSettingsCommand = new RelayCommand(ShowSettingsAction);

        ShowChatsCommand = new RelayCommand(ShowChatsAction);

        ShowPromptsCommand = new RelayCommand(ShowPromptsAction);
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

            if (storage.Theme is { })
            {
                Theme = storage.Theme;
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
            Theme = Theme,
        };
        await JsonSerializer.SerializeAsync(
            stream, 
            storage, s_serializerContext.StorageViewModel);
    }
}
