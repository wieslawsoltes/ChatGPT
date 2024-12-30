using System.Text.Json.Serialization;

namespace ChatGPT.ViewModels.Chat;

public partial class ChatFunctionCallViewModel : ViewModelBase
{
    [JsonConstructor]
    public ChatFunctionCallViewModel()
    {
    }

    public ChatFunctionCallViewModel(string name) 
        : this()
    {
        _name = name;
    }

    [JsonPropertyName("name")]
    public partial string? Name { get; set; }

    public ChatFunctionCallViewModel Copy()
    {
        return new ChatFunctionCallViewModel
        {
            Name = _name,
        };
    }
}
