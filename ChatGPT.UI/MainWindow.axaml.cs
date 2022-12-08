using System;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace ChatGPT.UI;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        
        Submit.Click += SubmitOnClick;
    }

    private async void SubmitOnClick(object? sender, RoutedEventArgs e)
    {
        if (string.IsNullOrEmpty(Prompt.Text))
        {
            return;
        }

        // Get the response data for the prompt
        var responseData = await ChatService.GetResponseDataAsync(Prompt.Text);

        // Print the response
        var choices = responseData.Choices;

        Choice.Text = choices?.FirstOrDefault()?.Text;
    }
}
