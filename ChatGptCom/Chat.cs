using System.Runtime.InteropServices;
using AI.Model.Services;
using AI.Services;
using ChatGPT;
using ChatGPT.Model.Services;
using ChatGPT.Services;
using ChatGPT.ViewModels.Chat;
using ChatGPT.ViewModels.Settings;
using Microsoft.Extensions.DependencyInjection;

namespace ChatGptCom;

[ComVisible(true)]
[Guid("8403C952-E751-4DE1-BD91-F35DEE19206E")]
[InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
public interface IChatEvents
{
    [DispId(1)]
    void OnSendCompleted();
}

[ComVisible(true)]
[Guid("BDAC53A6-3896-4A4B-A33B-98F44D667F62")]
[InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
public interface IChat
{
    [DispId(1)]
    Task SendAsync(string directions, string message);

    [DispId(2)]
    string? Result { get; set; }
}

[ComVisible(true)]
[Guid("B95D2D28-7BF7-458F-86BE-4B797FB70DBA")]
[ClassInterface(ClassInterfaceType.None)]
[ComSourceInterfaces(typeof(IChatEvents))]
[ProgId("ChatGptCom.Chat")]
public class Chat : IChat
{
    public delegate void SendCompletedDelegate();

    public event SendCompletedDelegate? OnSendCompleted;

    public string? Result { get; set; }

    static Chat()
    {
        IServiceCollection serviceCollection = new ServiceCollection();

        serviceCollection.AddSingleton<IStorageFactory, IsolatedStorageFactory>();
        serviceCollection.AddSingleton<IChatSerializer, NewtonsoftChatSerializer>();
        serviceCollection.AddSingleton<IChatService, ChatService>();

        serviceCollection.AddTransient<ChatMessageViewModel>();
        serviceCollection.AddTransient<ChatSettingsViewModel>();
        serviceCollection.AddTransient<ChatResultViewModel>();
        serviceCollection.AddTransient<ChatViewModel>();
        serviceCollection.AddTransient<PromptViewModel>();

        IServiceProvider serviceProvider = serviceCollection.BuildServiceProvider();

        Defaults.Locator.ConfigureServices(serviceProvider);
    }

    public async Task SendAsync(string directions, string message)
    {
        try
        {
            using var cts = new CancellationTokenSource();
            var chat = new ChatViewModel(directions);
            chat.AddSystemMessage(directions);
            chat.AddUserMessage(message);
            var chatResult = await chat.SendAsync(chat.CreateChatMessages(), cts.Token);
            var result = chatResult?.Message;
            if(result is { })
            {
                Result = result;
                OnSendCompleted?.Invoke();
            }
        }
        catch (Exception e)
        {
            Result = $"{e.Message}{Environment.NewLine}{e.StackTrace}";
            OnSendCompleted?.Invoke();
            Console.WriteLine(e);
        }
    }
}
