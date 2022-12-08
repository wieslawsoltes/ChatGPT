
namespace ChatGPT;

class Program
{
    static async Task Main(string[] args)
    {
        // Get the response data for the prompt
        var responseData = await ChatService.GetResponseDataAsync("Generate mockup for login page using xaml");

        // Print the response
        var choices = responseData.Choices;

        foreach (var choice in choices)
        {
            Console.WriteLine(choice.Text);
        }
    }
}
