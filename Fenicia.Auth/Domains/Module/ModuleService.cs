using System.Net;

using AutoMapper;

using Fenicia.Common;
using Fenicia.Common.Enums;

namespace Fenicia.Auth.Domains.Module;

public class ModuleService(
    IMapper mapper,
    ILogger<ModuleService> logger,
    IModuleRepository moduleRepository
) : IModuleService
{
    public async Task<ApiResponse<List<ModuleResponse>>> GetAllOrderedAsync(
        int page = 1,
        int perPage = 10
    )
    {
        logger.LogInformation("Getting all modules");
        var modules = await moduleRepository.GetAllOrderedAsync(page, perPage);

        var response = mapper.Map<List<ModuleResponse>>(modules);

        return new ApiResponse<List<ModuleResponse>>(response);
    }

    public async Task<ApiResponse<List<ModuleResponse>>> GetModulesToOrderAsync(
        IEnumerable<Guid> request
    )
    {
        logger.LogInformation("Getting modules to order");
        var modules = await moduleRepository.GetManyOrdersAsync(request);

        var response = mapper.Map<List<ModuleResponse>>(modules);

        return new ApiResponse<List<ModuleResponse>>(response);
    }

    public async Task<ApiResponse<ModuleResponse>> GetModuleByTypeAsync(ModuleType moduleType)
    {
        logger.LogInformation("Getting module by type {moduleType}", moduleType);
        var module = await moduleRepository.GetModuleByTypeAsync(moduleType);

        if (module is null)
        {
            return new ApiResponse<ModuleResponse>(
                null,
                HttpStatusCode.NotFound,
                TextConstants.ItemNotFound
            );
        }

        var response = mapper.Map<ModuleResponse>(module);

        return new ApiResponse<ModuleResponse>(response);
    }

    public async Task<ApiResponse<int>> CountAsync()
    {
        var response = await moduleRepository.CountAsync();

        return new ApiResponse<int>(response);
    }
}
