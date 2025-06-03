using Fenicia.Auth.Responses;
using Fenicia.Common.Enums;

namespace Fenicia.Auth.Services.Interfaces;

public interface IModuleService
{
    Task<List<ModuleResponse>> GetAllOrderedAsync();
    Task<List<ModuleResponse>> GetModulesToOrderAsync(IEnumerable<Guid> request);
    Task<ModuleResponse?> GetModuleByTypeAsync(ModuleType moduleType);
}