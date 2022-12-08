using System.Text;
using System.Text.Json;

namespace ChatGPT;

public class ChatService
{
    public static async Task<CompletionsResponse> GetResponseDataAsync(string prompt, decimal temperature, int maxTokens)
    {
        // Set up the API URL and API key
        string apiUrl = "https://api.openai.com/v1/completions";
        string? apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY");

        // Get the request body JSON
        string requestBodyJson = GetRequestBodyJson(prompt, temperature, maxTokens);

        // Send the API request and get the response data
        return await SendApiRequestAsync(apiUrl, apiKey, requestBodyJson);
    }

    private static string GetRequestBodyJson(string prompt, decimal temperature, int maxTokens)
    {
        // Set up the request body
        var requestBody = new CompletionsRequestBody
        {
            Model = "text-davinci-003",
            Prompt = prompt,
            Temperature = temperature,
            MaxTokens = maxTokens,
            TopP = 1.0m,
            FrequencyPenalty = 0.0m,
            PresencePenalty = 0.0m,
            N = 1,
            Stop = "[END]",
        };

        // Create a new JsonSerializerOptions object with the IgnoreNullValues and IgnoreReadOnlyProperties properties set to true
        var serializerOptions = new JsonSerializerOptions
        {
            IgnoreNullValues = true,
            IgnoreReadOnlyProperties = true,
        };

        // Serialize the request body to JSON using the JsonSerializer.Serialize method overload that takes a JsonSerializerOptions parameter
        return JsonSerializer.Serialize(requestBody, serializerOptions);
    }

    private static async Task<CompletionsResponse> SendApiRequestAsync(string apiUrl, string apiKey, string requestBodyJson)
    {
        // Create a new HttpClient for making the API request
        using HttpClient client = new HttpClient();

        // Set the API key in the request headers
        client.DefaultRequestHeaders.Add("Authorization", "Bearer " + apiKey);

        // Create a new StringContent object with the JSON payload and the correct content type
        StringContent content = new StringContent(requestBodyJson, Encoding.UTF8, "application/json");

        // Send the API request and get the response
        HttpResponseMessage response = await client.PostAsync(apiUrl, content);

        // Deserialize the response
        var responseBody = await response.Content.ReadAsStringAsync();

        // Return the response data
        return JsonSerializer.Deserialize<CompletionsResponse>(responseBody);
    }
}
