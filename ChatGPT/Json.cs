using System.Text.Json.Serialization;

namespace ChatGPT;

// https://beta.openai.com/docs/api-reference/completions/create

public class CompletionsRequestBody
{
    [JsonPropertyName("model")]
    public string? Model { get; set; }

    [JsonPropertyName("prompt")]
    public string? Prompt { get; set; }

    [JsonPropertyName("suffix")]
    public string? Suffix { get; set; }

    [JsonPropertyName("max_tokens")]
    public int MaxTokens { get; set; } = 16;

    [JsonPropertyName("temperature")]
    public decimal Temperature { get; set; } = 1;

    [JsonPropertyName("top_p")]
    public decimal TopP { get; set; } = 1;

    [JsonPropertyName("n")]
    public int N { get; set; } = 1;

    [JsonPropertyName("stream")]
    public bool Stream { get; set; } = false;

    [JsonPropertyName("logprobs")]
    public int? Logprobs { get; set; } = null;

    [JsonPropertyName("echo")]
    public bool Echo { get; set; } = false;

    [JsonPropertyName("stop")]
    public string? Stop { get; set; }

    [JsonPropertyName("presence_penalty")]
    public decimal PresencePenalty { get; set; }

    [JsonPropertyName("frequency_penalty")]
    public decimal FrequencyPenalty { get; set; }

    [JsonPropertyName("best_of")]
    public int BestOf { get; set; } = 1;

    [JsonPropertyName("logit_bias")]
    public Dictionary<string, decimal>? LogitBias { get; set; }

    [JsonPropertyName("user")]
    public string? User { get; set; }
}

public class CompletionsResponse
{
    [JsonPropertyName("id")]
    public string? Id { get; set; }

    [JsonPropertyName("object")]
    public string? Object { get; set; } // Escaped with @ symbol

    [JsonPropertyName("created")]
    public int Created { get; set; }

    [JsonPropertyName("model")]
    public string? Model { get; set; }

    [JsonPropertyName("choices")]
    public CompletionsChoice[]? Choices { get; set; }

    [JsonPropertyName("usage")]
    public CompletionsUsage? Usage { get; set; }
}

public class CompletionsChoice
{
    [JsonPropertyName("text")]
    public string? Text { get; set; }

    [JsonPropertyName("index")]
    public int Index { get; set; }

    [JsonPropertyName("logprobs")]
    public object? Logprobs { get; set; }

    [JsonPropertyName("finish_reason")]
    public string? FinishReason { get; set; }
}

public class CompletionsUsage
{
    [JsonPropertyName("prompt_tokens")]
    public int PromptTokens { get; set; }

    [JsonPropertyName("completion_tokens")]
    public int CompletionTokens { get; set; }

    [JsonPropertyName("total_tokens")]
    public int TotalTokens { get; set; }
}
