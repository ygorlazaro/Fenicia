using AutoMapper;
using Fenicia.Auth.Repositories.Interfaces;
using Fenicia.Auth.Responses;
using Fenicia.Auth.Services.Interfaces;
using Fenicia.Common.Enums;

namespace Fenicia.Auth.Services;

public class ModuleService(IMapper mapper, IModuleRepository moduleRepository) : IModuleService
{
    public async Task<List<ModuleResponse>> GetAllOrderedAsync()
    {
        var modules = await moduleRepository.GetAllOrderedAsync();
        
        return mapper.Map<List<ModuleResponse>>(modules);
    }

    public async Task<List<ModuleResponse>> GetModulesToOrderAsync(IEnumerable<Guid> request)
    {
        var modules =  await moduleRepository.GetManyOrdersAsync(request);
        
        return mapper.Map<List<ModuleResponse>>(modules);
    }

    public async Task<ModuleResponse?> GetModuleByTypeAsync(ModuleType moduleType)
    {
        var module = await moduleRepository.GetModuleByTypeAsync(moduleType);

        return module is null ? null : mapper.Map<ModuleResponse>(module);
    }
}