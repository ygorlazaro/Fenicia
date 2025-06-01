using Fenicia.Auth.Contexts.Models;
using Fenicia.Common.Enums;

namespace Fenicia.Auth.Services.Interfaces;

public interface IModuleService
{
    Task<List<ModuleModel>> GetAllOrderedAsync();
    Task<List<ModuleModel>> GetModulesToOrderAsync(IEnumerable<Guid> request);
    Task<ModuleModel?> GetModuleByTypeAsync(ModuleType moduleType);
}