namespace Fenicia.Auth.Domains.Module;

using System.Net;

using Common;
using Common.Database.Responses;
using Common.Enums;

using Fenicia.Common.Database.Models.Auth;

public class ModuleService : IModuleService
{
    private readonly ILogger<ModuleService> _logger;
    private readonly IModuleRepository _moduleRepository;

    public ModuleService(ILogger<ModuleService> logger, IModuleRepository moduleRepository)
    {
        _logger = logger;
        _moduleRepository = moduleRepository;
    }

    public async Task<ApiResponse<List<ModuleResponse>>> GetAllOrderedAsync(CancellationToken cancellationToken, int page = 1, int perPage = 10)
    {
        try
        {
            _logger.LogInformation("Getting all modules with page {Page} and items per page {PerPage}", page, perPage);

            var modules = await _moduleRepository.GetAllOrderedAsync(cancellationToken, page, perPage);
            _logger.LogDebug("Retrieved {Count} modules from repository", modules.Count);

            var mapped = ModuleResponse.Convert(modules);

            return new ApiResponse<List<ModuleResponse>>(mapped);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while getting all modules");
            throw;
        }
    }

    public async Task<ApiResponse<List<ModuleResponse>>> GetModulesToOrderAsync(IEnumerable<Guid> request, CancellationToken cancellationToken)
    {
        try
        {
            var enumerable = request as Guid[] ?? [.. request];
            _logger.LogInformation("Getting modules to order for {Count} module IDs", enumerable.Length);

            var modules = await _moduleRepository.GetManyOrdersAsync(enumerable, cancellationToken);
            _logger.LogDebug("Retrieved {Count} modules from repository", modules.Count);

            var response = ModuleResponse.Convert(modules);

            return new ApiResponse<List<ModuleResponse>>(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while getting modules to order");
            throw;
        }
    }

    public async Task<ApiResponse<ModuleResponse>> GetModuleByTypeAsync(ModuleType moduleType, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Getting module by type {ModuleType}", moduleType);

            var module = await _moduleRepository.GetModuleByTypeAsync(moduleType, cancellationToken);

            if (module is null)
            {
                _logger.LogWarning("Module not found for type {ModuleType}", moduleType);
                return new ApiResponse<ModuleResponse>(data: null, HttpStatusCode.NotFound, TextConstants.ItemNotFound);
            }

            _logger.LogDebug("Module found for type {ModuleType}", moduleType);
            var response = ModuleResponse.Convert(module);
            return new ApiResponse<ModuleResponse>(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while getting module by type {ModuleType}", moduleType);
            throw;
        }
    }

    public async Task<ApiResponse<int>> CountAsync(CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Counting total number of modules");

            var response = await _moduleRepository.CountAsync(cancellationToken);
            _logger.LogDebug("Total module count: {Count}", response);

            return new ApiResponse<int>(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while counting modules");
            throw;
        }
    }

    public async Task<ApiResponse<List<ModuleResponse>>> LoadModulesAtDatabaseAsync(CancellationToken cancellationToken)
    {
        var modulesToSave = new List<ModuleModel>
                            {
                                new() { Name = "ERP", Amount = -1, Type = ModuleType.Erp },
                                new() { Name = "Auth", Amount = 10, Type = ModuleType.Auth },
                                new() { Name = "Basic", Amount = 20, Type = ModuleType.Basic },
                                new() { Name = "Social Network", Amount = 20, Type = ModuleType.SocialNetwork },
                                new() { Name = "Project", Amount = 20, Type = ModuleType.Project },
                                new() { Name = "Performance Evaluation", Amount = 20, Type = ModuleType.PerformanceEvaluation },
                                new() { Name = "Accounting", Amount = 20, Type = ModuleType.Accounting },
                                new() { Name = "HR", Amount = 20, Type = ModuleType.Hr },
                                new() { Name = "POS", Amount = 20, Type = ModuleType.Pos },
                                new() { Name = "Contracts", Amount = 20, Type = ModuleType.Contracts },
                                new() { Name = "Ecommerce", Amount = 20, Type = ModuleType.Ecommerce },
                                new() { Name = "Customer Support", Amount = 20, Type = ModuleType.CustomerSupport },
                                new() { Name = "Plus", Amount = 20, Type = ModuleType.Plus }
                            };

        var response = await _moduleRepository.LoadModulesAtDatabaseAsync(modulesToSave, cancellationToken);
        var mapped = ModuleResponse.Convert(response);

        return new ApiResponse<List<ModuleResponse>>(mapped);
    }

    public async Task<ApiResponse<List<ModuleResponse>>> GetUserModulesAsync(Guid userId, Guid companyId, CancellationToken cancellationToken)
    {
        var userModules = await _moduleRepository.GetUserModulesAsync(userId, companyId, cancellationToken);

        var mapped = ModuleResponse.Convert(userModules);

        return new ApiResponse<List<ModuleResponse>>(mapped);
    }
}
