using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using AI.Model.Json.Chat;
using AI.Model.Services;

namespace AI.Services;

public class ChatService : IChatService
{
    private static readonly HttpClient s_client;

    private static readonly ChatJsonContext s_serializerContext;

    static ChatService()
    {
        s_client = new();

        s_serializerContext = new(
            new JsonSerializerOptions
            {
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                IgnoreReadOnlyProperties = true
            });
    }

    private static string GetRequestBodyJson(ChatServiceSettings settings)
    {
        // Set up the request body
        var requestBody = new ChatRequestBody
        {
            Model = settings.Model,
            Messages = settings.Messages,
            MaxTokens = settings.MaxTokens,
            Temperature = settings.Temperature,
            TopP = settings.TopP,
            N = 1,
            Stream = false,
            Stop = settings.Stop,
            FrequencyPenalty = settings.FrequencyPenalty,
            PresencePenalty = settings.PresencePenalty,
            User = null
        };

        // Serialize the request body to JSON using the JsonSerializer.
        return JsonSerializer.Serialize(requestBody, s_serializerContext.ChatRequestBody);
    }

    private static async Task<ChatResponse?> SendApiRequestAsync(string apiUrl, string apiKey, string requestBodyJson, CancellationToken token)
    {
        // Create a new HttpClient for making the API request

        // Set the API key in the request headers
        if (s_client.DefaultRequestHeaders.Contains("Authorization"))
        {
            s_client.DefaultRequestHeaders.Remove("Authorization");
        }
        s_client.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");

        // Create a new StringContent object with the JSON payload and the correct content type
        var content = new StringContent(requestBodyJson, Encoding.UTF8, "application/json");

        // Send the API request and get the response
        var response = await s_client.PostAsync(apiUrl, content, token);

        // Deserialize the response
#if NETFRAMEWORK
        var responseBody = await response.Content.ReadAsStringAsync();
#else
        var responseBody = await response.Content.ReadAsStringAsync(token);
#endif
        switch (response.StatusCode)
        {
            case HttpStatusCode.Unauthorized:
#if !NETFRAMEWORK
            case HttpStatusCode.TooManyRequests:
#endif
            case HttpStatusCode.InternalServerError:
            {
                return JsonSerializer.Deserialize(responseBody, s_serializerContext.ChatResponseError);
            }
        }

        if (response.StatusCode != HttpStatusCode.OK)
        {
            return null;
        }

        // Return the response data
        return JsonSerializer.Deserialize(responseBody, s_serializerContext.ChatResponseSuccess);
    }

    public async Task<ChatResponse?> GetResponseDataAsync(ChatServiceSettings settings, CancellationToken token)
    {
        // Set up the API URL and API key
        var apiUrl = "https://api.openai.com/v1/chat/completions";
        var apiKey = Environment.GetEnvironmentVariable(Constants.EnvironmentVariableApiKey);
        if (apiKey is null)
        {
            return null;
        }

        // Get the request body JSON
        var requestBodyJson = GetRequestBodyJson(settings);

        // Send the API request and get the response data
        return await SendApiRequestAsync(apiUrl, apiKey, requestBodyJson, token);
    }
}
