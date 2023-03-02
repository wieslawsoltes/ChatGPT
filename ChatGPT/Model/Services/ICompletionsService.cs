using System.Threading.Tasks;
using ChatGPT.Model.Json;

namespace ChatGPT.Model.Services;

public interface ICompletionsService
{
    Task<CompletionsResponse?> GetResponseDataAsync(CompletionsServiceSettings settings);
}
