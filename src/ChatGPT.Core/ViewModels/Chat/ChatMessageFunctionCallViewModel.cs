using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using CommunityToolkit.Mvvm.ComponentModel;

namespace ChatGPT.ViewModels.Chat;

public class ChatMessageFunctionCallViewModel : ObservableObject
{
    private string? _name;
    private Dictionary<string, object>? _arguments;

    [JsonConstructor]
    public ChatMessageFunctionCallViewModel()
    {
    }

    public ChatMessageFunctionCallViewModel(string role, Dictionary<string, object> arguments) 
        : this()
    {
        _name = role;
        _arguments = arguments;
    }

    [JsonPropertyName("name")]
    public string? Name
    {
        get => _name;
        set => SetProperty(ref _name, value);
    }

    [JsonPropertyName("arguments")]
    public Dictionary<string, object>? Arguments
    {
        get => _arguments;
        set => SetProperty(ref _arguments, value);
    }

    public ChatMessageFunctionCallViewModel Copy()
    {
        var functionCall = new ChatMessageFunctionCallViewModel
        {
            Name = _name,
            // TODO: Copy entry Value if it's reference value.
            Arguments = _arguments?.ToDictionary(
                e => e.Key, 
                e => e.Value)
        };

        return functionCall;
    }
}
