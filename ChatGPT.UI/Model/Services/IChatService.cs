using System.Threading.Tasks;
using ChatGPT.UI.Model.Json;

namespace ChatGPT.UI.Model.Services;

public interface IChatService
{
    Task<CompletionsResponse?> GetResponseDataAsync(string prompt, decimal temperature, int maxTokens);
}
