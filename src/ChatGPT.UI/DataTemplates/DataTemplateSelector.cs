using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Markup.Xaml.Templates;

namespace ChatGPT.DataTemplates;

public class DataTemplateSelector : StyledElement, IDataTemplate
{
    public static readonly StyledProperty<object?> SelectorProperty = 
        AvaloniaProperty.Register<DataTemplateSelector, object?>(nameof(Selector));

    public static readonly StyledProperty<object?> DefaultSelectorProperty = 
        AvaloniaProperty.Register<DataTemplateSelector, object?>(nameof(DefaultSelector));

    public static readonly StyledProperty<string?> TemplateSuffixProperty = 
        AvaloniaProperty.Register<DataTemplateSelector, string?>(nameof(TemplateSuffix), "DataTemplate");

    public object? Selector
    {
        get => GetValue(SelectorProperty);
        set => SetValue(SelectorProperty, value);
    }

    public object? DefaultSelector
    {
        get => GetValue(DefaultSelectorProperty);
        set => SetValue(DefaultSelectorProperty, value);
    }

    public string? TemplateSuffix
    {
        get => GetValue(TemplateSuffixProperty);
        set => SetValue(TemplateSuffixProperty, value);
    }

    public Control Build(object? data)
    {
        if (data is null)
        {
            return new TextBlock { Text = "Data is null." };
        }

        var selector = Selector ?? DefaultSelector;
        var suffix = TemplateSuffix;
        var key = $"{selector}{suffix}";

        this.TryFindResource(key, null, out var resource);

        if (resource is null)
        {
            Application.Current?.TryFindResource(key, null, out resource);
        }

        if (resource is DataTemplate dataTemplate)
        {
            var control = dataTemplate.Build(data);
            if (control is { })
            {
                return control;
            }

            return new TextBlock { Text = "Failed to build control: " + data };
        }

        return new TextBlock { Text = "Data template not found: " + selector };
    }

    public bool Match(object? data)
    {
        return data is not null;
    }
}
