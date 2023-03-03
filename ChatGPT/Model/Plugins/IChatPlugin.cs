using System.Threading.Tasks;

namespace ChatGPT.Model.Plugins;

public interface IChatPlugin
{
    string Id { get; }
    string Name { get; }
    Task Start();
    Task Stop();
    Task Initialize(IPluginContext context);
    Task Shutdown();
}
