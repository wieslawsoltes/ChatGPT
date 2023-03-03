using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using ChatGPT.Model.Plugins;
using ChatGPT.Model.Services;
using ChatGPT.ViewModels.Chat;
using ChatGPT.ViewModels.Settings;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Input;

namespace ChatGPT.ViewModels;

public class MainViewModel : ObservableObject, IPluginContext
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

    private ObservableCollection<ChatViewModel> _chats;
    private ChatViewModel? _currentChat;
    private string? _theme;

    public MainViewModel()
    {
        _chats = new ObservableCollection<ChatViewModel>();

        NewCallback();

        AddChatCommand = new AsyncRelayCommand(NewAction);

        DeleteChatCommand = new AsyncRelayCommand(DeleteAction);

        OpenChatCommand = new AsyncRelayCommand(OpenAction);

        SaveChatCommand = new AsyncRelayCommand(SaveAction);

        ExportChatCommand = new AsyncRelayCommand(ExportAction);

        DefaultChatSettingsCommand = new RelayCommand(DefaultChatSettingsAction);

        ExitCommand = new RelayCommand(ExitAction);
        
        ChangeThemeCommand = new RelayCommand(ChangeThemeAction);
    }

    [JsonPropertyName("chats")]
    public ObservableCollection<ChatViewModel> Chats
    {
        get => _chats;
        set => SetProperty(ref _chats, value);
    }

    [JsonPropertyName("currentChat")]
    public ChatViewModel? CurrentChat
    {
        get => _currentChat;
        set => SetProperty(ref _currentChat, value);
    }

    [JsonPropertyName("theme")]
    public string? Theme
    {
        get => _theme;
        set => SetProperty(ref _theme, value);
    }

    [JsonIgnore]
    public IAsyncRelayCommand AddChatCommand { get; }

    [JsonIgnore]
    public IAsyncRelayCommand DeleteChatCommand { get; }

    [JsonIgnore]
    public IAsyncRelayCommand OpenChatCommand { get; }

    [JsonIgnore]
    public IAsyncRelayCommand SaveChatCommand { get; }

    [JsonIgnore]
    public IAsyncRelayCommand ExportChatCommand { get; }

    [JsonIgnore]
    public IRelayCommand DefaultChatSettingsCommand { get; }

    [JsonIgnore]
    public IRelayCommand ExitCommand { get; }

    [JsonIgnore]
    public IRelayCommand ChangeThemeCommand { get; }

    private ChatSettingsViewModel CreateDefaultChatSettings()
    {
        return new ChatSettingsViewModel
        {
            Temperature = Defaults.DefaultTemperature,
            MaxTokens = Defaults.DefaultMaxTokens,
            Model = Defaults.DefaultModel,
            ApiKey = null,
            Directions = Defaults.DefaultDirections,
            Format = Defaults.MarkdownMessageFormat,
        };
    }

    private async Task NewAction()
    {
        NewCallback();
        await Task.Yield();
    }

    private async Task DeleteAction()
    {
        DeleteCallback();
        await Task.Yield();
    }

    private async Task OpenAction()
    {
        var app = Ioc.Default.GetService<IApplicationService>();
        if (app is { })
        {
            await app.OpenFile(
                OpenCallbackAsync, 
                new List<string>(new[] { "Json", "All" }), 
                "Open");
        }
    }

    private async Task SaveAction()
    {
        var app = Ioc.Default.GetService<IApplicationService>();
        if (app is { } && CurrentChat is { })
        {
            await app.SaveFile(
                SaveCallbackAsync, 
                new List<string>(new[] { "Json", "All" }), 
                "Save", 
                CurrentChat.Name ?? "chat", 
                "json");
        }
    }

    private async Task ExportAction()
    {
        var app = Ioc.Default.GetService<IApplicationService>();
        if (app is { } && CurrentChat is { })
        {
            await app.SaveFile(
                ExportCallbackAsync, 
                new List<string>(new[] { "Text", "All" }), 
                "Export", 
                CurrentChat.Name ?? "chat",
                "txt");
        }
    }

    private async Task CopyAction()
    {
        var app = Ioc.Default.GetService<IApplicationService>();
        if (app is { } && CurrentChat is { })
        {
            var sb = new StringBuilder();
            await using var writer = new StringWriter(sb);
            await ExportAsync(CurrentChat, writer);
            await app.SetClipboardText(sb.ToString());
        }
    }

    private void DefaultChatSettingsAction()
    {
        if (CurrentChat is { } chat)
        {
            var apiKey = chat.Settings?.ApiKey;

            chat.Settings = CreateDefaultChatSettings();

            if (apiKey is { })
            {
                chat.Settings.ApiKey = apiKey;
            }
        }
    }

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

    private void NewCallback()
    {
        var chat = new ChatViewModel
        {
            Name = "Chat",
            Settings = CurrentChat?.Settings?.Copy() ?? CreateDefaultChatSettings()
        };

        var welcomeItem = new ChatMessageViewModel
        {
            Prompt = "",
            Message = "Hi! I'm Clippy, your Windows Assistant. Would you like to get some assistance?",
            Format = Defaults.TextMessageFormat,
            IsSent = false,
            CanRemove = false
        };
        chat.SetMessageActions(welcomeItem);
        chat.Messages.Add(welcomeItem);
        chat.CurrentMessage = welcomeItem;

        Chats.Add(chat);
        CurrentChat = chat;
    }

    private void DeleteCallback()
    {
        if (CurrentChat is { })
        {
            Chats.Remove(CurrentChat);
            CurrentChat = Chats.LastOrDefault();
        }
    }

    private async Task OpenCallbackAsync(Stream stream)
    {
        var chat = await JsonSerializer.DeserializeAsync(
            stream, 
            s_serializerContext.ChatViewModel);
        if (chat is { })
        {
            foreach (var message in chat.Messages)
            {
                chat.SetMessageActions(message);
            }

            Chats.Add(chat);
            CurrentChat = chat;
        }
    }

    private async Task SaveCallbackAsync(Stream stream)
    {
        if (CurrentChat is null)
        {
            return;
        }

        await JsonSerializer.SerializeAsync(
            stream, 
            CurrentChat, s_serializerContext.ChatViewModel);
    }

    private async Task ExportCallbackAsync(Stream stream)
    {
        if (CurrentChat is null)
        {
            return;
        }

        await using var writer = new StreamWriter(stream);
        await ExportAsync(CurrentChat, writer);
    }

    private async Task ExportAsync(ChatViewModel chat, TextWriter writer)
    {
        if (chat.Settings?.Directions is { } directions)
        {
            await writer.WriteLineAsync("SYSTEM:");
            await writer.WriteLineAsync("");

            await writer.WriteLineAsync(directions);
            await writer.WriteLineAsync("");
        }

        for (var i = 0; i < chat.Messages.Count; i++)
        {
            var message = chat.Messages[i];

            if (i < 1)
            {
                continue;
            }

            if (message.Result is { })
            {
                if (!string.IsNullOrEmpty(message.Message))
                {
                    await writer.WriteLineAsync("USER:");
                    await writer.WriteLineAsync("");

                    await writer.WriteLineAsync(message.Message);
                    await writer.WriteLineAsync("");
                }

                if (!string.IsNullOrEmpty(message.Result.Message))
                {
                    await writer.WriteLineAsync("ASSISTANT:");
                    await writer.WriteLineAsync("");

                    await writer.WriteLineAsync(message.Result.Message);
                    await writer.WriteLineAsync("");
                }
            }
        }
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
            Theme = Theme,
        };
        await JsonSerializer.SerializeAsync(
            stream, 
            storage, s_serializerContext.StorageViewModel);
    }
}
