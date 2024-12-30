using System.Text.Json.Serialization;

namespace ChatGPT.ViewModels.Layouts;

public partial class WindowLayoutViewModel : ViewModelBase
{
    [JsonConstructor]
    public WindowLayoutViewModel()
    {
    }

    [JsonPropertyName("x")]
    public partial int X { get; set; }

    [JsonPropertyName("y")]
    public partial int Y { get; set; }

    [JsonPropertyName("width")]
    public partial double Width { get; set; }

    [JsonPropertyName("height")]
    public partial double Height { get; set; }

    [JsonPropertyName("windowState")]
    public partial string? WindowState { get; set; }

    [JsonPropertyName("windowStartupLocation")]
    public partial string? WindowStartupLocation { get; set; }

    [JsonPropertyName("topmost")]
    public partial bool Topmost { get; set; }
    
    public WindowLayoutViewModel Copy()
    {
        return new WindowLayoutViewModel
        {
            X = _x,
            Y = _y,
            Width = _width,
            Height = _height,
            WindowState = _windowState,
            WindowStartupLocation = _windowStartupLocation,
            Topmost = _topmost,
        };
    }
}
