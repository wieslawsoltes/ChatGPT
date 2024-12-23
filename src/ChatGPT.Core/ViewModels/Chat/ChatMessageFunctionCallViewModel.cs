using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace ChatGPT.ViewModels.Chat;

public partial class ChatMessageFunctionCallViewModel : ViewModelBase
{
    [JsonConstructor]
    public ChatMessageFunctionCallViewModel()
    {
    }

    public ChatMessageFunctionCallViewModel(string role, Dictionary<string, string> arguments) 
        : this()
    {
        _name = role;
        _arguments = arguments;
    }

    [JsonPropertyName("name")]
    public partial string? Name { get; set; }

    [JsonPropertyName("arguments")]
    public partial Dictionary<string, string>? Arguments { get; set; }

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
