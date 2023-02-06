using ChatGPT;

var prompt = "Generate mockup for login page using xaml";
var temperature = 0.6m;
var maxTokens = 10;
var responseData = await ChatService.GetResponseDataAsync(prompt, temperature, maxTokens);

var choices = responseData?.Choices;
if (choices != null)
{
    foreach (var choice in choices)
    {
        Console.WriteLine(choice.Text);
    }
}
