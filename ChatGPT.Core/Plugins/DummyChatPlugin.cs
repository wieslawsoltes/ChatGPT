using System.Threading.Tasks;
using ChatGPT.Model.Plugins;

namespace ChatGPT.Plugins;

public class DummyChatPlugin : IChatPlugin
{
    public string Id => "Dummy";

    public string Name => "Dummy";

    public async Task Start()
    {
        await Task.Yield();
    }

    public async Task Stop()
    {
        await Task.Yield();
    }

    public async Task Initialize(IPluginContext context)
    {
        await Task.Yield();
    }

    public async Task Shutdown()
    {
        await Task.Yield();
    }
}
