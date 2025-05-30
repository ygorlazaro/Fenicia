using Fenicia.Auth.Contexts.Models;
using Fenicia.Auth.Enums;
using Fenicia.Auth.Repositories.Interfaces;
using Fenicia.Auth.Services.Interfaces;

namespace Fenicia.Auth.Services;

public class ModuleService(IModuleRepository moduleRepository) : IModuleService
{
    public async Task<List<ModuleModel>> GetAllOrderedAsync()
    {
        return await moduleRepository.GetAllOrderedAsync();
    }

    public async Task<List<ModuleModel?>> GetModulesToOrderAsync(IEnumerable<Guid> request)
    {
        return await moduleRepository.GetManyOrdersAsync(request);
    }

    public async Task<ModuleModel?> GetModuleByTypeAsync(ModuleType moduleType)
    {
        return await moduleRepository.GetModuleByTypeAsync(moduleType);
    }
}