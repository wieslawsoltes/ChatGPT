using System.Runtime.InteropServices;

namespace ChatGptCom;

[ComVisible(true)]
[Guid("BDAC53A6-3896-4A4B-A33B-98F44D667F62")]
[InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
public interface IChat
{
    [DispId(1)]
    void Create(string? directions, int maxTokens = 2000, string model = "gpt-3.5-turbo");

    [DispId(2)]
    Task MessageAsync(string message, string role = "user", bool send = true);

    [DispId(3)]
    Task AskAsync(string directions, string message);

    [DispId(4)]
    string? Result { get; set; }
}
