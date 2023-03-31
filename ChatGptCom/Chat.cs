using System.Runtime.InteropServices;
using ChatGPT;
using ChatGPT.ViewModels.Chat;

namespace ChatGptCom;

[ComVisible(true)]
[Guid("66F5D400-3F8E-4206-91DD-2F48459EE7E8")]
[InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
public interface IChat
{
    string? Send(string directions, string message);
}

[ComVisible(true)]
[Guid("B95D2D28-7BF7-458F-86BE-4B797FB70DBA")]
[ClassInterface(ClassInterfaceType.None)]
public class Chat : IChat
{
    static Chat()
    {
        Defaults.ConfigureDefaultServices();
    }

    public string? Send(string directions, string message)
    {
        using var cts = new CancellationTokenSource();
        var chat = new ChatViewModel(directions);
        chat.AddSystemMessage(directions);
        chat.AddUserMessage(message);
        var result = chat.SendAsync(chat.CreateChatMessages(), cts.Token).Result;
        return result?.Message;
    }
}
