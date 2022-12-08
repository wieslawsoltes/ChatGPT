using System;
using System.Globalization;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace ChatGPT.UI;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();

        Temperature.Text = "0.6";

        MaxTokens.Text = "10";

        Submit.Click += SubmitOnClick;
    }

    private async void SubmitOnClick(object? sender, RoutedEventArgs e)
    {
        if (string.IsNullOrEmpty(Prompt.Text))
        {
            return;
        }

        IsEnabled = false;

        // Get the response data for the prompt
        string prompt = Prompt.Text;
        decimal temperature = decimal.Parse(Temperature.Text, CultureInfo.InvariantCulture);
        int maxTokens  = int.Parse(MaxTokens.Text, CultureInfo.InvariantCulture);
        var responseData = await ChatService.GetResponseDataAsync(prompt, temperature, maxTokens);

        // Print the response
        var choices = responseData.Choices;

        Choice.Text = choices?.FirstOrDefault()?.Text;

        IsEnabled = true;
    }
}
