using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Input.Platform;
using ChatGPT.Model.Plugins;

namespace ChatGPT.Plugins;

public class ClipboardListenerChatPlugin : IChatPlugin
{
    private IPluginContext? _context;
    private CancellationTokenSource? _cts;
    private bool _sync;
    private string? _text;
    private readonly ConcurrentQueue<string> _queue = new();
    private bool _skipFirst = true;

    public string Id => "ClipboardListener";

    public string Name => "Clipboard Listener";

    public async Task StartAsync()
    {
        Run();

        await Task.Yield();
    }

    public async Task StopAsync()
    {
        Cancel();

        await Task.Yield();
    }

    public async Task InitializeAsync(IPluginContext context)
    {
        _context = context;

        await Task.Yield();
    }

    public async Task ShutdownAsync()
    {
        Cancel();

        await Task.Yield();
    }

    private void Run()
    {
        if (_cts is not null)
        {
            return;
        }

        _cts = new CancellationTokenSource();

#pragma warning disable CS4014
        Task.Run(async () =>
#pragma warning restore CS4014
        {
            await RunTaskPeriodicallyAsync(
                _cts.Token,
                CheckClipboardTextAsync,
                TimeSpan.FromMilliseconds(20));
        });

#pragma warning disable CS4014
        Task.Run(async () =>
#pragma warning restore CS4014
        {
            await RunTaskPeriodicallyAsync(
                _cts.Token,
                RunSendPeriodicallyAsync,
                TimeSpan.FromMilliseconds(20));
        });
    }

    private void Cancel()
    {
        _cts?.Cancel();
        _cts = null;
    }

    private async Task RunTaskPeriodicallyAsync(CancellationToken ct, Func<CancellationToken, Task> func, TimeSpan interval)
    {
        while (true)
        {
            if (ct.IsCancellationRequested)
            {
                break;
            }
            await func(ct);
            await Task.Delay(interval, ct);
        }
    }

    private async Task CheckClipboardTextAsync(CancellationToken ct)
    {
        if (_sync)
        {
            return;
        }

        _sync = true;

        if (AvaloniaLocator.Current.GetService<IClipboard>() is { } clipboard)
        {
            var text = await clipboard.GetTextAsync();
            if (!string.IsNullOrEmpty(text) && _text != text)
            {
                _text = text;

                if (_skipFirst)
                {
                    _skipFirst = false;
                }
                else
                {
                    // Console.WriteLine($"[Clipboard] {text}");
                    _queue.Enqueue(text);
                }
            }
        }

        _sync = false;
    }

    private async Task RunSendPeriodicallyAsync(CancellationToken ct)
    {
        if (_context is null)
        {
            return;
        }

        while (true)
        {
            if (ct.IsCancellationRequested)
            {
                break;
            }

            if (_queue.TryDequeue(out var text))
            {
                // Console.WriteLine($"[Send] {text}");
                await Send(text);
            }
        }
    }
    
    private async Task Send(string prompt)
    {
        if (_context is null)
        {
            return;
        }

        if (_context.CurrentChat is {} chat)
        {
            // var message = new ChatMessageViewModel
            // {
            //     Prompt = prompt,
            //     Message = "Clipboard",
            //     Format = chat.Settings?.Format ?? Defaults.TextMessageFormat,
            //     IsSent = false,
            //     CanRemove = true
            // };
            // chat.SetMessageActions(message);
            // chat.Messages.Add(message);
            // chat.CurrentMessage = message;
            // await chat.Send(message);

            // Console.WriteLine($"[InsertPrompt] {prompt}, {chat.CurrentMessage}");

            var message = chat.CurrentMessage ?? chat.Messages.LastOrDefault();
            if (message is { })
            {
                message.Role = "user";
                message.Message = prompt;
                await chat.SendAsync(message, true);
            }
        }
    }
}
