using System;
using System.Runtime.InteropServices.JavaScript;
using System.Threading.Tasks;
using Avalonia;

namespace ChatGPT.UI.Browser.Services;

internal static partial class Interop
{
    [JSExport]
    internal static async Task SaveSettings()
    {
        if (Application.Current is App app)
        {
            Console.WriteLine("[.NET] Saving settings...");
            await app.SaveSettings();
            Console.WriteLine("[.NET] Saved settings.");
        }
    }
}
