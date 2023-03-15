using System.Text.Json.Serialization.Metadata;
using System.Threading.Tasks;

namespace ChatGPT.Model.Services;

public interface IStorageService<T>
{
    Task SaveObject(T obj, string key, JsonTypeInfo<T> typeInfo);
    Task<T?> LoadObject(string key, JsonTypeInfo<T> typeInfo);
}
