using System.Threading.Tasks;
using AI.Model.Json;

namespace AI.Model.Services;

public interface IChatService
{
    Task<ChatResponse?> GetResponseDataAsync(ChatServiceSettings settings);
}
