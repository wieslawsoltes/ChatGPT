using System.Text;
using System.Text.Json;

namespace ChatGPT;

class Program
{
    static async Task Main(string[] args)
    {
        // Set up the API URL and API key
        string apiUrl = "https://api.openai.com/v1/completions";
        string? apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY");

        // Set the prompt to use for the completions
        string query = "Generate mockup for login page using xaml";

        // Set the model to use for completion
        string model = "text-davinci-003";

        // Set the number of completions to generate
        int numCompletions = 1;

        // Set the temperature for the completions (0.0 - 1.0)
        float temperature = 0.6f;

        // Set the maximum token count for the completions
        int maxTokens = 10;

        // Set up the request body
        var requestBody = new
        {
            model = model,
            prompt = query,
            temperature = temperature,
            max_tokens = maxTokens,
            top_p = 1.0,
            frequency_penalty = 0.0,
            presence_penalty = 0.0,
            n = numCompletions,
            stop = "[END]",
        };

        // Serialize the request body to JSON
        var requestBodyJson = JsonSerializer.Serialize(requestBody);

        // Create a new HttpClient for making the API request
        using HttpClient client = new HttpClient();

        // Set the API key in the request headers
        client.DefaultRequestHeaders.Add("Authorization", "Bearer " + apiKey);

        // Create a new StringContent object with the JSON payload and the correct content type
        StringContent content = new StringContent(requestBodyJson, Encoding.UTF8, "application/json");

        // Send the API request and get the response
        HttpResponseMessage response = client.PostAsync(apiUrl, content).Result;

        // Deserialize the response
        var responseBody = await response.Content.ReadAsStringAsync();

        // Print the response
        var responseData = JsonDocument.Parse(responseBody).RootElement;

        // Print the response
        foreach (var choice in responseData.GetProperty("choices").EnumerateArray())
        {
            Console.WriteLine(choice.GetProperty("text").GetString());
        }
    }
}