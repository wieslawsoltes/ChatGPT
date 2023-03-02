using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using AI;
using AI.Model.Json;
using AI.Model.Services;
using ChatGPT.Model.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.DependencyInjection;

namespace ChatGPT.ViewModels;

public class ChatViewModel : ObservableObject
{
    private string? _name;
    private ChatSettingsViewModel? _settings;
    private ObservableCollection<ChatMessageViewModel> _messages;
    private ChatMessageViewModel? _currentMessage;
    private bool _isEnabled;

    public ChatViewModel()
    {
        _messages = new ObservableCollection<ChatMessageViewModel>();
        _isEnabled = true;
    }

    [JsonPropertyName("name")]
    public string? Name
    {
        get => _name;
        set => SetProperty(ref _name, value);
    }

    [JsonPropertyName("settings")]
    public ChatSettingsViewModel? Settings
    {
        get => _settings;
        set => SetProperty(ref _settings, value);
    }

    [JsonPropertyName("messages")]
    public ObservableCollection<ChatMessageViewModel> Messages
    {
        get => _messages;
        set => SetProperty(ref _messages, value);
    }

    [JsonPropertyName("currentMessage")]
    public ChatMessageViewModel? CurrentMessage
    {
        get => _currentMessage;
        set => SetProperty(ref _currentMessage, value);
    }

    [JsonIgnore]
    public bool IsEnabled
    {
        get => _isEnabled;
        set => SetProperty(ref _isEnabled, value);
    }

    public void SetMessageActions(ChatMessageViewModel message)
    {
        message.SetSendAction(Send);
        message.SetCopyAction(Copy);
        message.SetRemoveAction(Remove);
    }

    public async Task Copy(ChatMessageViewModel message)
    {
        var app = Ioc.Default.GetService<IApplicationService>();
        if (app is { })
        {
            if (message.Message is { } text)
            {
                await app.SetClipboardText(text);
            }
        }
    }

    public void Remove(ChatMessageViewModel message)
    {
        if (message is { CanRemove: true, IsAwaiting: false })
        {
            Messages.Remove(message);

            var lastMessage = Messages.LastOrDefault();
            if (lastMessage is { })
            {
                lastMessage.IsSent = false;
            }
        }
    }

    public async Task Send(ChatMessageViewModel sendMessage)
    {
        if (Settings is null)
        {
            return;
        }

        if (string.IsNullOrEmpty(sendMessage.Prompt))
        {
            return;
        }

        var chatPrompt = CreateChatPrompt(sendMessage, Messages, Settings);

        IsEnabled = false;

        try
        {
            sendMessage.IsSent = true;

            ChatMessageViewModel? promptMessage;
            ChatMessageViewModel? resultMessage = null;

            if (sendMessage.Result is { })
            {
                promptMessage = sendMessage;
                resultMessage = sendMessage.Result;
            }
            else
            {
                promptMessage = new ChatMessageViewModel
                {
                    CanRemove = true,
                    Format = Defaults.TextMessageFormat
                };
                SetMessageActions(promptMessage);
                Messages.Add(promptMessage);
            }

            var prompt = sendMessage.Prompt;

            promptMessage.Message = prompt;
            promptMessage.Prompt = "";
            promptMessage.IsSent = true;

            CurrentMessage = promptMessage;
            promptMessage.IsAwaiting = true;

            // Response

            var chatServiceSettings = new ChatServiceSettings
            {
                Model = Settings.Model,
                Messages = chatPrompt,
                Suffix = null,
                Temperature = Settings.Temperature,
                MaxTokens = Settings.MaxTokens,
                TopP = 1.0m,
                Stop = null,
            };
            var responseStr = default(string);
            var isResponseStrError = false;
            var responseData = await GetResponseData(chatServiceSettings, Settings);
            if (responseData is null)
            {
                responseStr = "Unknown error.";
                isResponseStrError = true;
            }
            else if (responseData is ChatResponseError error)
            {
                var message = error.Error?.Message;
                responseStr = message ?? "Unknown error.";
                isResponseStrError = true;
            }
            else if (responseData is ChatResponseSuccess success)
            {
                var message = success.Choices?.FirstOrDefault()?.Message?.Content?.Trim();
                responseStr = message ?? "";
                isResponseStrError = false;
            }

            // Update

            if (isResponseStrError)
            {
                resultMessage = promptMessage;
            }

            if (resultMessage is null)
            {
                resultMessage = new ChatMessageViewModel
                {
                    IsSent = false,
                    CanRemove = true,
                    Format = Settings.Format
                };
                SetMessageActions(resultMessage);
                Messages.Add(resultMessage);
            }
            else
            {
                if (!isResponseStrError)
                {
                    resultMessage.IsSent = true;
                }
            }

            resultMessage.Message = responseStr;
            resultMessage.IsError = isResponseStrError;
            resultMessage.Prompt = isResponseStrError ? prompt : "";
            resultMessage.Format = Settings.Format;

            if (Messages.LastOrDefault() == resultMessage)
            {
                resultMessage.IsSent = false;
            }

            CurrentMessage = resultMessage;

            promptMessage.IsAwaiting = false;
            promptMessage.Result = isResponseStrError ? null : resultMessage;
        }
        catch (Exception)
        {
            // ignored
        }

        IsEnabled = true;
    }

    private static ChatMessage[] CreateChatPrompt(
        ChatMessageViewModel sendMessage, 
        ObservableCollection<ChatMessageViewModel> messages, 
        ChatSettingsViewModel chatSettings)
    {
        var chatMessages = new List<ChatMessage>();

        chatMessages.Add(new ChatMessage
        {
            Role = "system",
            Content = chatSettings.Directions
        });

        // TODO: Ensure that chat prompt does not exceed maximum token limit.

        foreach (var message in messages)
        {
            if (!string.IsNullOrEmpty(message.Message) && message.Result is { })
            {
                chatMessages.Add(new ChatMessage
                {
                    Role = "user",
                    Content = message.Message
                });
                chatMessages.Add(new ChatMessage
                {
                    Role = "assistant",
                    Content = message.Result.Message
                });
            }
        }

        chatMessages.Add(new ChatMessage
        {
            Role = "user",
            Content = sendMessage.Prompt
        });

        return chatMessages.ToArray();
    }

    private static async Task<ChatResponse?> GetResponseData(ChatServiceSettings chatServiceSettings, ChatSettingsViewModel chatSettings)
    {
        var chat = Ioc.Default.GetService<IChatService>();
        if (chat is null)
        {
            return null;
        }

        var apiKey = Environment.GetEnvironmentVariable(Constants.EnvironmentVariableApiKey);
        var restoreApiKey = false;

        if (!string.IsNullOrWhiteSpace(chatSettings.ApiKey))
        {
            Environment.SetEnvironmentVariable(Constants.EnvironmentVariableApiKey, chatSettings.ApiKey);
            restoreApiKey = true;
        }

        ChatResponse? responseData = null;
        try
        {
            responseData = await chat.GetResponseDataAsync(chatServiceSettings);
        }
        catch (Exception)
        {
            // ignored
        }

        if (restoreApiKey && !string.IsNullOrWhiteSpace(apiKey))
        {
            Environment.SetEnvironmentVariable(Constants.EnvironmentVariableApiKey, apiKey);
        }

        return responseData;
    }
}
