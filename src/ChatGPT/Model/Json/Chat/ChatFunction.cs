using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace AI.Model.Json.Chat;

[DataContract]
public class ChatFunction
{
    [DataMember(Name = "name")]
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [DataMember(Name = "description")]
    [JsonPropertyName("description")]
    public string? Description { get; set; }

    // JSON Schema object
    // https://json-schema.org/understanding-json-schema/
    [DataMember(Name = "parameters")]
    [JsonPropertyName("parameters")]
    public object? Parameters { get; set; }
}
