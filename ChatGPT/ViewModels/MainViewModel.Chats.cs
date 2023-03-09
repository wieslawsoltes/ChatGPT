using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using ChatGPT.Model.Services;
using ChatGPT.ViewModels.Chat;
using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Input;

namespace ChatGPT.ViewModels;

public partial class MainViewModel
{
    private ObservableCollection<ChatViewModel> _chats;
    private ChatViewModel? _currentChat;

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
    public IAsyncRelayCommand CopyChatCommand { get; }

    [JsonIgnore]
    public IRelayCommand DefaultChatSettingsCommand { get; }

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

    private async Task AddChatAction()
    {
        NewChatCallback();
        await Task.Yield();
    }

    private async Task DeleteChatAction()
    {
        DeleteChatCallback();
        await Task.Yield();
    }

    private async Task OpenChatAction()
    {
        var app = Ioc.Default.GetService<IApplicationService>();
        if (app is { })
        {
            await app.OpenFile(
                OpenChatCallbackAsync, 
                new List<string>(new[] { "Json", "All" }), 
                "Open");
        }
    }

    private async Task SaveChatAction()
    {
        var app = Ioc.Default.GetService<IApplicationService>();
        if (app is { } && CurrentChat is { })
        {
            await app.SaveFile(
                SaveChatCallbackAsync, 
                new List<string>(new[] { "Json", "All" }), 
                "Save", 
                CurrentChat.Name ?? "chat", 
                "json");
        }
    }

    private async Task ExportChatAction()
    {
        var app = Ioc.Default.GetService<IApplicationService>();
        if (app is { } && CurrentChat is { })
        {
            await app.SaveFile(
                ExportChatCallbackAsync, 
                new List<string>(new[] { "Text", "All" }), 
                "Export", 
                CurrentChat.Name ?? "chat",
                "txt");
        }
    }

    private async Task CopyChatAction()
    {
        var app = Ioc.Default.GetService<IApplicationService>();
        if (app is { } && CurrentChat is { })
        {
            var sb = new StringBuilder();
            await using var writer = new StringWriter(sb);
            await ExportChatAsync(CurrentChat, writer);
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

    private void NewChatCallback()
    {
        var chat = new ChatViewModel
        {
            Name = "Chat",
            Settings = CurrentChat?.Settings?.Copy() ?? CreateDefaultChatSettings()
        };

        var welcomeItem = new ChatMessageViewModel
        {
            Role = "system",
            Message = "Hi! I'm Clippy, your Windows Assistant. Would you like to get some assistance?",
            Format = Defaults.TextMessageFormat,
            IsSent = true,
            CanRemove = false
        };
        chat.SetMessageActions(welcomeItem);
        chat.Messages.Add(welcomeItem);

        var promptItem = new ChatMessageViewModel
        {
            Role = "user",
            Message = "",
            Format = Defaults.TextMessageFormat,
            IsSent = false,
            CanRemove = false
        };
        chat.SetMessageActions(promptItem);
        chat.Messages.Add(promptItem);

        chat.CurrentMessage = promptItem;

        Chats.Add(chat);
        CurrentChat = chat;
    }

    private void DeleteChatCallback()
    {
        if (CurrentChat is { })
        {
            Chats.Remove(CurrentChat);
            CurrentChat = Chats.LastOrDefault();
        }
    }

    private async Task OpenChatCallbackAsync(Stream stream)
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

    private async Task SaveChatCallbackAsync(Stream stream)
    {
        if (CurrentChat is null)
        {
            return;
        }

        await JsonSerializer.SerializeAsync(
            stream, 
            CurrentChat, s_serializerContext.ChatViewModel);
    }

    private async Task ExportChatCallbackAsync(Stream stream)
    {
        if (CurrentChat is null)
        {
            return;
        }

        await using var writer = new StreamWriter(stream);
        await ExportChatAsync(CurrentChat, writer);
    }

    private async Task ExportChatAsync(ChatViewModel chat, TextWriter writer)
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
}
