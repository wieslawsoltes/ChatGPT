using System.Collections.Generic;
using ChatGPT.Model.Plugins;
using ChatGPT.Model.Services;
using ChatGPT.Plugins;

namespace ChatGPT.Services;

public class PluginsService : IPluginsService
{
    private readonly List<IChatPlugin> _plugins = new();
    private readonly IPluginContext _pluginContext;

    public PluginsService(IPluginContext pluginContext)
    {
        _pluginContext = pluginContext;
    }

    public void DiscoverPlugins()
    {
        // TODO:
        _plugins.Add(new ClipboardListenerChatPlugin());
    }

    public void InitPlugins()
    {
        foreach (var plugin in _plugins)
        {
            plugin.Initialize(_pluginContext);
        }
    }

    public void StartPlugins()
    {
        foreach (var plugin in _plugins)
        {
            plugin.Start();
        }
    }

    public void ShutdownPlugins()
    {
        foreach (var plugin in _plugins)
        {
            plugin.Shutdown();
        }
    }
}
