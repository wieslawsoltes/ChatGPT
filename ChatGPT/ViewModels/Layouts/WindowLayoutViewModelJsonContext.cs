using System.Text.Json.Serialization;

namespace ChatGPT.ViewModels.Layouts;

[JsonSerializable(typeof(WindowLayoutViewModel))]
public partial class WindowLayoutViewModelJsonContext : JsonSerializerContext
{
}
