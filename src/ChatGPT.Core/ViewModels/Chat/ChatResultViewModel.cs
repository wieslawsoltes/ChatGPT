using System.Text.Json.Serialization;

namespace ChatGPT.ViewModels.Chat;

public partial class ChatResultViewModel : ViewModelBase
{
    [JsonPropertyName("name")]
    public partial string? Message { get; set; }

    [JsonPropertyName("isError")]
    public partial bool IsError { get; set; }

    [JsonPropertyName("function_call")]
    public partial ChatMessageFunctionCallViewModel? FunctionCall { get; set; }
}
