using System.Runtime.InteropServices;
using AI.Services;
using ChatGPT.ViewModels.Chat;
using ChatGptCom.Services;

namespace ChatGptCom;

[ComVisible(true)]
[Guid("B95D2D28-7BF7-458F-86BE-4B797FB70DBA")]
[ClassInterface(ClassInterfaceType.None)]
[ComSourceInterfaces(typeof(IChatEvents))]
[ProgId("ChatGptCom.Chat")]
public class Chat : IChat
{
    private ChatViewModel? _chat;
    private readonly NewtonsoftChatSerializer _chatSerializer;
    private readonly ChatService _chatService;

    public delegate void SendCompletedDelegate();

    public event SendCompletedDelegate? OnSendCompleted;

    public string? Result { get; set; }

    public Chat()
    {
        _chatSerializer = new NewtonsoftChatSerializer();
        _chatService = new ChatService(_chatSerializer);
    }

    private async Task Send(ChatViewModel chat)
    {
        using var cts = new CancellationTokenSource();
        var chatResult = await chat.SendAsync(chat.CreateChatMessages(), cts.Token);
        var result = chatResult?.Message;
        if (result is { })
        {
            chat.AddAssistantMessage(result);
            Result = result;
            OnSendCompleted?.Invoke();
        }
    }

    public void Create(string? directions, int maxTokens = 2000, string model = "gpt-3.5-turbo")
    {
        _chat = new ChatViewModel(
            _chatService,
            _chatSerializer,
            new ChatSettingsViewModel
            {
                MaxTokens = maxTokens,
                Model = model
            });

        if (directions is { })
        {
            _chat.AddSystemMessage(directions);
        }
    }

    public async Task MessageAsync(string message, string role = "user", bool send = true)
    {
        if (_chat is null)
        {
            return;
        }

        try
        {
            switch (role)
            {
                case "system":
                    _chat.AddSystemMessage(message);
                    break;
                case "user":
                    _chat.AddUserMessage(message);
                    break;
                case "assistant":
                    _chat.AddAssistantMessage(message);
                    break;
                default:
                    throw new ArgumentException($"Invalid {nameof(role)}.");
            }

            if (send)
            {
                await Send(_chat);
            }
        }
        catch (Exception e)
        {
            Result = $"{e.Message}{Environment.NewLine}{e.StackTrace}";
            OnSendCompleted?.Invoke();
            Console.WriteLine(e);
        }
    }

    public async Task AskAsync(string directions, string message)
    {
        try
        {
            var chat = new ChatViewModel(_chatService, _chatSerializer, directions);
            chat.AddSystemMessage(directions);
            chat.AddUserMessage(message);
            await Send(chat);
        }
        catch (Exception e)
        {
            Result = $"{e.Message}{Environment.NewLine}{e.StackTrace}";
            OnSendCompleted?.Invoke();
            Console.WriteLine(e);
        }
    }
}
