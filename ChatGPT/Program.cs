using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;

namespace ChatGPT
{
    class Program
    {
        static async Task Main(string[] args)
        {
            // Set up the API URL and API key
            string apiUrl = "https://api.openai.com/v1/completions";
            string? apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY");

            // Get the request body JSON
            string requestBodyJson = GetRequestBodyJson("Generate mockup for login page using xaml");

            // Send the API request and get the response data
            var responseData = await SendApiRequestAsync(apiUrl, apiKey, requestBodyJson);
            
            // Print the response
            foreach (var choice in responseData.GetProperty("choices").EnumerateArray())
            {
                Console.WriteLine(choice.GetProperty("text").GetString());
            }
        }

        private static string GetRequestBodyJson(string prompt)
        {
            // Set up the request body
            var requestBody = new RequestBody
            {
                model = "text-davinci-003",
                prompt = prompt,
                temperature = 0.6m,
                max_tokens = 10,
                top_p = 1.0m,
                frequency_penalty = 0.0m,
                presence_penalty = 0.0m,
                n = 1,
                stop = "[END]",
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

        private static async Task<JsonElement> SendApiRequestAsync(string apiUrl, string apiKey, string requestBodyJson)
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
            return JsonDocument.Parse(responseBody).RootElement;
        }
    }
}
