using System.Runtime.InteropServices;

namespace ChatGptCom;

[ComVisible(true)]
[Guid("8403C952-E751-4DE1-BD91-F35DEE19206E")]
[InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
public interface IChatEvents
{
    [DispId(1)]
    void OnSendCompleted();
}
