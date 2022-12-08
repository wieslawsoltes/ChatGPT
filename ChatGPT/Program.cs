
namespace ChatGPT;

class Program
{
    static async Task Main(string[] args)
    {
        // Get the response data for the prompt
        string prompt = "Generate mockup for login page using xaml";
        decimal temperature = 0.6m;
        int maxTokens = 10;
        var responseData = await ChatService.GetResponseDataAsync(prompt, temperature, maxTokens);

        // Print the response
        var choices = responseData.Choices;

        foreach (var choice in choices)
        {
            Console.WriteLine(choice.Text);
        }
    }
}
