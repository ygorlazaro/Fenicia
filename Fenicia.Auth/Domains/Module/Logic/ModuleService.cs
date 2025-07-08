namespace Fenicia.Auth.Domains.Module.Logic;

using System.Net;

using Common;
using Common.Enums;

using Data;

public class ModuleService(ILogger<ModuleService> logger, IModuleRepository moduleRepository) : IModuleService
{
    public async Task<ApiResponse<List<ModuleResponse>>> GetAllOrderedAsync(CancellationToken cancellationToken, int page = 1, int perPage = 10)
    {
        try
        {
            const string key = "modules";

            logger.LogInformation(message: "Getting all modules with page {Page} and items per page {PerPage}", page, perPage);

            var modules = await moduleRepository.GetAllOrderedAsync(cancellationToken, page, perPage);
            logger.LogDebug(message: "Retrieved {Count} modules from repository", modules.Count);

            var mapped = ModuleResponse.Convert(modules);

            return new ApiResponse<List<ModuleResponse>>(mapped);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, message: "Error occurred while getting all modules");
            throw;
        }
    }

    public async Task<ApiResponse<List<ModuleResponse>>> GetModulesToOrderAsync(IEnumerable<Guid> request, CancellationToken cancellationToken)
    {
        try
        {
            var enumerable = request as Guid[] ?? request.ToArray();
            logger.LogInformation(message: "Getting modules to order for {Count} module IDs", enumerable.Length);

            var modules = await moduleRepository.GetManyOrdersAsync(enumerable, cancellationToken);
            logger.LogDebug(message: "Retrieved {Count} modules from repository", modules.Count);

            var response = ModuleResponse.Convert(modules);

            return new ApiResponse<List<ModuleResponse>>(response);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, message: "Error occurred while getting modules to order");
            throw;
        }
    }

    public async Task<ApiResponse<ModuleResponse>> GetModuleByTypeAsync(ModuleType moduleType, CancellationToken cancellationToken)
    {
        try
        {
            logger.LogInformation(message: "Getting module by type {ModuleType}", moduleType);

            var module = await moduleRepository.GetModuleByTypeAsync(moduleType, cancellationToken);

            if (module is null)
            {
                logger.LogWarning(message: "Module not found for type {ModuleType}", moduleType);
                return new ApiResponse<ModuleResponse>(data: null, HttpStatusCode.NotFound, TextConstants.ItemNotFound);
            }

            logger.LogDebug(message: "Module found for type {ModuleType}", moduleType);
            var response = ModuleResponse.Convert(module);
            return new ApiResponse<ModuleResponse>(response);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, message: "Error occurred while getting module by type {ModuleType}", moduleType);
            throw;
        }
    }

    public async Task<ApiResponse<int>> CountAsync(CancellationToken cancellationToken)
    {
        try
        {
            logger.LogInformation(message: "Counting total number of modules");

            var response = await moduleRepository.CountAsync(cancellationToken);
            logger.LogDebug(message: "Total module count: {Count}", response);

            return new ApiResponse<int>(response);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, message: "Error occurred while counting modules");
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

        var response = await moduleRepository.LoadModulesAtDatabaseAsync(modulesToSave, cancellationToken);
        var mapped = ModuleResponse.Convert(response);

        return new ApiResponse<List<ModuleResponse>>(mapped);
    }
}
