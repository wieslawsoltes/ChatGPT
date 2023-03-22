# ChatGPT

[![CI](https://github.com/wieslawsoltes/ChatGPT/actions/workflows/build.yml/badge.svg?branch=main)](https://github.com/wieslawsoltes/ChatGPT/actions/workflows/build.yml)

[![NuGet](https://img.shields.io/nuget/v/ChatGPT.svg)](https://www.nuget.org/packages/ChatGPT)
[![NuGet](https://img.shields.io/nuget/dt/ChatGPT.svg)](https://www.nuget.org/packages/ChatGPT)

A ChatGPT C# client for graphical user interface runs on MacOS, Windows, Linux, Android, iOS and Browser. Powered by [Avalonia UI](https://avaloniaui.net/) framework.

To make the app work, you need to set the [OpenAI API key](https://beta.openai.com/account/api-keys) as the `OPENAI_API_KEY` environment variable or set API key directly in app settings.

You can try client using browser version [here](https://wieslawsoltes.github.io/ChatGPT/).

![image](https://user-images.githubusercontent.com/2297442/224843834-a58190df-3bdb-4722-b737-94e7adc87805.png)

# Shortcuts

### Main Window

- Ctrl+Shift+A - Toggle between transparent and acrylic blur window style.
- Ctrl+Shift+S - Toggle between visible and hidden window state.

### Message Prompt

- Enter - Send prompt.
- Escape - Cancel edit.
- F2 - Edit prompt.
- Shift+Enter, Alt+Enter - Insert new line.

# Build

1. Install [.NET 7.0](https://dotnet.microsoft.com/en-us/download/dotnet/7.0)
2. Run `dotnet workload install ios android wasm-tools` command
3. `dotnet publish -c Release` command inside project directory (mobile/desktop) or `dotnet run` for desktop to just run

### Dependencies

- [Avalonia](https://github.com/AvaloniaUI/Avalonia)
- [Markdown.Avalonia](https://github.com/whistyun/Markdown.Avalonia)
- [Avalonia.HtmlRenderer](https://github.com/AvaloniaUI/Avalonia.HtmlRenderer)
- [CommunityToolkit.Mvvm](https://github.com/CommunityToolkit/dotnet)
- [Microsoft.Extensions.DependencyInjection](https://www.nuget.org/packages/Microsoft.Extensions.DependencyInjection/)

# NuGet

- [ChatGPT](https://www.nuget.org/packages/ChatGPT) - An OpenAI api library for .NET.
- [ChatGPT.Core](https://www.nuget.org/packages/ChatGPT.Core) - An OpenAI client core library for .NET.
- [ChatGPT.UI](https://www.nuget.org/packages/ChatGPT.UI) - An OpenAI client user interface library for .NET.

# Docs

- [Guide Chat completions](https://platform.openai.com/docs/guides/chat)
- [API Reference](https://platform.openai.com/docs/api-reference/chat)

# License

ChatGPT is licensed under the [MIT license](LICENSE).
