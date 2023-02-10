using System.Threading.Tasks;
using ChatGPT.Model.Json;

namespace ChatGPT.Model.Services;

public interface IChatService
{
    Task<CompletionsResponse?> GetResponseDataAsync(ChatServiceSettings settings);
}
