using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Markup.Xaml.Templates;
using ChatGPT.ViewModels;

namespace ChatGPT.DataTemplates;

public class MessageDataTemplate : StyledElement, IDataTemplate
{
    public Control Build(object? data)
    {
        if (data is not MessageViewModel messageViewModel)
        {
            return new TextBlock { Text = "Invalid data type." };
        }

        var format = messageViewModel.Format ?? "Text";
        var key = $"{format}MessageTemplate";
        this.TryFindResource(key, out var resource);
        if (resource is DataTemplate dataTemplate)
        {
            return dataTemplate.Build(data);
        }

        return new TextBlock { Text = "Message template not found for format: " + format };
    }

    public bool Match(object? data)
    {
        return data is MessageViewModel;
    }
}
