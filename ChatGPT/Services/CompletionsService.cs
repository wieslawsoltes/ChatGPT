using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using ChatGPT.Model.Json;
using ChatGPT.Model.Services;

namespace ChatGPT.Services;

public class CompletionsService : ICompletionsService
{
    private static readonly HttpClient s_client = new();

    private static readonly CompletionsJsonContext s_serializerContext = new(
        new JsonSerializerOptions
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            IgnoreReadOnlyProperties = true
        });

    private static string GetRequestBodyJson(CompletionsServiceSettings settings)
    {
        // Set up the request body
        var requestBody = new CompletionsRequestBody
        {
            Model = settings.Model,
            Prompt = settings.Prompt,
            Suffix = null,
            MaxTokens = settings.MaxTokens,
            Temperature = settings.Temperature,
            TopP = settings.TopP,
            N = 1,
            Stream = false,
            Stop = settings.Stop,
            FrequencyPenalty = 0.0m,
            PresencePenalty = 0.0m,
            User = null
        };

        // Serialize the request body to JSON using the JsonSerializer.
        return JsonSerializer.Serialize(requestBody, s_serializerContext.CompletionsRequestBody);
    }

    private static async Task<CompletionsResponse?> SendApiRequestAsync(string apiUrl, string apiKey, string requestBodyJson)
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
        var response = await s_client.PostAsync(apiUrl, content);

        // Deserialize the response
        var responseBody = await response.Content.ReadAsStringAsync();

        switch (response.StatusCode)
        {
            case HttpStatusCode.Unauthorized:
            case HttpStatusCode.TooManyRequests:
            case HttpStatusCode.InternalServerError:
            {
                return JsonSerializer.Deserialize(responseBody, s_serializerContext.CompletionsResponseError);
            }
        }

        if (response.StatusCode != HttpStatusCode.OK)
        {
            return null;
        }

        // Return the response data
        return JsonSerializer.Deserialize(responseBody, s_serializerContext.CompletionsResponseSuccess);
    }

    public async Task<CompletionsResponse?> GetResponseDataAsync(CompletionsServiceSettings settings)
    {
        // Set up the API URL and API key
        var apiUrl = "https://api.openai.com/v1/completions";
        var apiKey = Environment.GetEnvironmentVariable(Constants.EnvironmentVariableApiKey);
        if (apiKey is null)
        {
            return null;
        }

        // Get the request body JSON
        var requestBodyJson = GetRequestBodyJson(settings);

        // Send the API request and get the response data
        return await SendApiRequestAsync(apiUrl, apiKey, requestBodyJson);
    }
}
