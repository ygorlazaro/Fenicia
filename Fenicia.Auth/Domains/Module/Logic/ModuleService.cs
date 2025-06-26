using System.Net;

using AutoMapper;

using Fenicia.Auth.Domains.Module.Data;
using Fenicia.Common;
using Fenicia.Common.Enums;

namespace Fenicia.Auth.Domains.Module.Logic;

public class ModuleService(
    IMapper mapper,
    ILogger<ModuleService> logger,
    IModuleRepository moduleRepository
) : IModuleService
{
    public async Task<ApiResponse<List<ModuleResponse>>> GetAllOrderedAsync(
        CancellationToken cancellationToken, int page = 1,
        int perPage = 10)
    {
        logger.LogInformation("Getting all modules");
        var modules = await moduleRepository.GetAllOrderedAsync(cancellationToken, page, perPage);

        var response = mapper.Map<List<ModuleResponse>>(modules);

        return new ApiResponse<List<ModuleResponse>>(response);
    }

    public async Task<ApiResponse<List<ModuleResponse>>> GetModulesToOrderAsync(
        IEnumerable<Guid> request,
        CancellationToken cancellationToken
    )
    {
        logger.LogInformation("Getting modules to order");
        var modules = await moduleRepository.GetManyOrdersAsync(request, cancellationToken);

        var response = mapper.Map<List<ModuleResponse>>(modules);

        return new ApiResponse<List<ModuleResponse>>(response);
    }

    public async Task<ApiResponse<ModuleResponse>> GetModuleByTypeAsync(ModuleType moduleType, CancellationToken cancellationToken)
    {
        logger.LogInformation("Getting module by type {moduleType}", moduleType);
        var module = await moduleRepository.GetModuleByTypeAsync(moduleType, cancellationToken);

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

    public async Task<ApiResponse<int>> CountAsync(CancellationToken cancellationToken)
    {
        var response = await moduleRepository.CountAsync(cancellationToken);

        return new ApiResponse<int>(response);
    }
}
