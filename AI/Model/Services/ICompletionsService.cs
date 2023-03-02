using System.Threading.Tasks;
using AI.Model.Json;

namespace AI.Model.Services;

public interface ICompletionsService
{
    Task<CompletionsResponse?> GetResponseDataAsync(CompletionsServiceSettings settings);
}
