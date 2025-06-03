using AutoMapper;
using Fenicia.Auth.Repositories.Interfaces;
using Fenicia.Auth.Responses;
using Fenicia.Auth.Services.Interfaces;
using Fenicia.Common.Enums;

namespace Fenicia.Auth.Services;

public class ModuleService(IMapper mapper, ILogger<ModuleService> logger, IModuleRepository moduleRepository) : IModuleService
{
    public async Task<List<ModuleResponse>> GetAllOrderedAsync()
    {
        logger.LogInformation("Getting all modules");
        var modules = await moduleRepository.GetAllOrderedAsync();
        
        return mapper.Map<List<ModuleResponse>>(modules);
    }

    public async Task<List<ModuleResponse>> GetModulesToOrderAsync(IEnumerable<Guid> request)
    {
        logger.LogInformation("Getting modules to order");
        var modules =  await moduleRepository.GetManyOrdersAsync(request);
        
        return mapper.Map<List<ModuleResponse>>(modules);
    }

    public async Task<ModuleResponse?> GetModuleByTypeAsync(ModuleType moduleType)
    {
        logger.LogInformation("Getting module by type {moduleType}", [moduleType]);
        var module = await moduleRepository.GetModuleByTypeAsync(moduleType);

        return module is null ? null : mapper.Map<ModuleResponse>(module);
    }
}