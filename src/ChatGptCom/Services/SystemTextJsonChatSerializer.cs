using AI.Model.Services;
using Newtonsoft.Json;

namespace ChatGptCom.Services;

public class NewtonsoftChatSerializer : IChatSerializer
{
    private static readonly JsonSerializerSettings s_settings;

    static NewtonsoftChatSerializer()
    {
        s_settings = new JsonSerializerSettings
        {
            Formatting = Formatting.Indented,
            NullValueHandling = NullValueHandling.Ignore,
        };
    }

    public string Serialize<T>(T value)
    {
        return JsonConvert.SerializeObject(value, s_settings);
    }

    public T? Deserialize<T>(string json)
    {
        return JsonConvert.DeserializeObject<T>(json, s_settings);
    }
}
