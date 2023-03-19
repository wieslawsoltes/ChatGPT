import { dotnet } from './dotnet.js'
import { registerAvaloniaModule } from './avalonia.js';

const is_browser = typeof window != "undefined";
if (!is_browser) throw new Error(`Expected to be running in a browser`);

const dotnetRuntime = await dotnet
    .withDiagnosticTracing(false)
    .withApplicationArgumentsFromQuery()
    .create();

await registerAvaloniaModule(dotnetRuntime);

const config = dotnetRuntime.getConfig();
const exports = await dotnetRuntime.getAssemblyExports(config.mainAssemblyName);

exports.ChatGPT.UI.Browser.Services.Interop.SaveSettings();

window.addEventListener('beforeunload',  (event) => {
    console.log("[JS] Saving settings...");
    exports.Interop.SaveSettings();
    console.log("[JS] Saved settings.");
    event.preventDefault();
    event.returnValue = true;
});

const terminationEvent = 'onpagehide' in self ? 'pagehide' : 'unload';

window.addEventListener(terminationEvent,  (event) => {
    console.log("[JS] Saving settings...");
    exports.Interop.SaveSettings();
    console.log("[JS] Saved settings.");
});

await dotnetRuntime.runMainAndExit(config.mainAssemblyName, [window.location.search]);
