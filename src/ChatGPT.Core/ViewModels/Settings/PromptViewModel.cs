using System.Text.Json.Serialization;

namespace ChatGPT.ViewModels.Settings;

public partial class PromptViewModel : ViewModelBase
{
    [JsonPropertyName("act")]
    public partial string? Act { get; set; }

    [JsonPropertyName("prompt")]
    public partial string? Prompt { get; set; }

    public PromptViewModel Copy()
    {
        return new PromptViewModel
        {
            Act = _act,
            Prompt = _prompt
        };
    }
}
