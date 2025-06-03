using Fenicia.Auth.Responses;
using Fenicia.Common.Enums;

namespace Fenicia.Auth.Services.Interfaces;

public interface IModuleService
{
    Task<List<ModuleResponse>> GetAllOrderedAsync(int page = 1, int perPage = 10);
    Task<List<ModuleResponse>> GetModulesToOrderAsync(IEnumerable<Guid> request);
    Task<ModuleResponse?> GetModuleByTypeAsync(ModuleType moduleType);
    Task<int> CountAsync();
}