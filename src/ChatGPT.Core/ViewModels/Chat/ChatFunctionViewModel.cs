using System.Text.Json.Serialization;

namespace ChatGPT.ViewModels.Chat;

public partial class ChatFunctionViewModel : ViewModelBase
{
    [JsonConstructor]
    public ChatFunctionViewModel()
    {
    }

    public ChatFunctionViewModel(string name, string description) 
        : this()
    {
        _name = name;
        _description = description;
    }

    public ChatFunctionViewModel(string name, string description, object parameters) 
        : this()
    {
        _name = name;
        _description = description;
        _parameters = parameters;
    }

    [JsonPropertyName("name")]
    public partial string? Name { get; set; }

    [JsonPropertyName("description")]
    public partial string? Description { get; set; }

    [JsonPropertyName("parameters")]
    public partial object? Parameters { get; set; }

    public ChatFunctionViewModel Copy()
    {
        return new ChatFunctionViewModel
        {
            Name = _name,
            Description = _description,
            // TODO: Copy Parameters if type is reference.
            Parameters = _parameters
        };
    }
}
