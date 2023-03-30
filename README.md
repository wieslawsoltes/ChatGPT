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

# .NET tool

Install:
```bash
dotnet tool install --global ChatGPT.CLI --version 1.0.0-preview.7
```

Uninstall:
```bash
dotnet tool uninstall --global ChatGPT.CLI
```

- [ChatGPT.CLI](https://www.nuget.org/packages/ChatGPT.CLI) - An .NET ChatGPT tool.

### Usage

```
ChatGPT.CLI:
An .NET ChatGPT tool.

Usage:
ChatGPT.CLI [options]

Options:
-f, --inputFiles <inputfiles>              The relative or absolute path to the input files
-d, --inputDirectory <inputdirectory>      The relative or absolute path to the input directory
-o, --outputDirectory <outputdirectory>    The relative or absolute path to the output directory
--outputFiles <outputfiles>                The relative or absolute path to the output files
-p, --pattern <pattern>                    The search string to match against the names of files in the input directory
-r, --recursive                            Recurse into subdirectories of input directory search
-e, --extension <extension>                The output file extension
-s, --settingsFile <settingsfile>          The relative or absolute path to the settings file
--temperature <temperature>                What sampling temperature to use, between 0 and 2. Higher values like 0.8 will make the output more random, while lower values like 0.2 will make it more focused and deterministic.
--topP <topp>                              An alternative to sampling with temperature, called nucleus sampling, where the model considers the results of the tokens with top_p probability mass. So 0.1 means only the tokens comprising the top 10% probability mass are considered.
--presencePenalty <presencepenalty>        Number between -2.0 and 2.0. Positive values penalize new tokens based on whether they appear in the text so far, increasing the model's likelihood to talk about new topics.
--frequencyPenalty <frequencypenalty>      Number between -2.0 and 2.0. Positive values penalize new tokens based on their existing frequency in the text so far, decreasing the model's likelihood to repeat the same line verbatim.
--maxTokens <maxtokens>                    The maximum number of tokens to generate in the chat completion.
--apiKey <apikey>                          Override OpenAI api key. By default OPENAI_API_KEY environment variable is used.
--model <model>                            ID of the model to use. See the model endpoint compatibility table for details on which models work with the Chat API.
--directions <directions>                  The system message (directions) helps set the behavior of the assistant. Typically, a conversation is formatted with a system message first, followed by alternating user and assistant messages.
-t, --threads <threads>                    The number of parallel job threads
--quiet                                    Set verbosity level to quiet
--version                                  Show version information
-?, -h, --help                             Show help and usage information
```

### Examples

- Using .NET tool `chat` command:

C# to VB
```bash
chat -d ./ -e vb -p *.cs --directions "You are C# to VB conversion expert. Convert input code from C# to VB. Write only converted code."
```

C# to F#
```bash
chat -d ./ -e fs -p *.cs --directions "You are C# to F# conversion expert. Convert input code from C# to F#. Write only code."
```

Refactor C# code
```bash
chat -d ./ -e cs -p *.cs --directions "You are C# expert. Refactor C# code to use fluent api. Write only code."
```

Write API documentation
```bash
chat -d ./ -e md -p *.cs --directions "You are a technical documentation writer. Write API documentation for C# code. If XML docs are missing write them."
```

- Run from source

C# to VB
```bash
dotnet run -- -d ./ -e vb -p *.cs --directions "You are C# to VB conversion expert. Convert input code from C# to VB. Write only converted code."
```

C# to F#
```bash
dotnet run -- -d ./ -e fs -p *.cs --directions "You are C# to F# conversion expert. Convert input code from C# to F#. Write only code."
```

Write API documentation
```bash
dotnet run -- -d ./ -e md -p *.cs --directions "You are a technical documentation writer. Write API documentation for C# code. If XML docs are missing write them."
```

### Settings file format

```json
{
    "temperature": 0.7,
    "top_p": 1,
    "presence_penalty": 0,
    "frequency_penalty": 0,
    "maxTokens": 2000,
    "apiKey": "",
    "model": "gpt-3.5-turbo",
    "directions": "You are a helpful assistant."
}
```

# NuGet

- [ChatGPT](https://www.nuget.org/packages/ChatGPT) - An OpenAI api library for .NET.
- [ChatGPT.Core](https://www.nuget.org/packages/ChatGPT.Core) - An OpenAI client core library for .NET.
- [ChatGPT.UI](https://www.nuget.org/packages/ChatGPT.UI) - An OpenAI client user interface library for .NET.

# Docs

- [Guide Chat completions](https://platform.openai.com/docs/guides/chat)
- [API Reference](https://platform.openai.com/docs/api-reference/chat)

# License

ChatGPT is licensed under the [MIT license](LICENSE).
