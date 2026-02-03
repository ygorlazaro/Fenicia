using System.Net;

using Fenicia.Common;
using Fenicia.Common.Database.Models.Auth;
using Fenicia.Common.Database.Responses;
using Fenicia.Common.Enums;

namespace Fenicia.Auth.Domains.Module;

public class ModuleService(IModuleRepository moduleRepository) : IModuleService
{
    public async Task<ApiResponse<List<ModuleResponse>>> GetAllOrderedAsync(CancellationToken cancellationToken, int page = 1, int perPage = 10)
    {
        var modules = await moduleRepository.GetAllOrderedAsync(cancellationToken, page, perPage);
        var mapped = ModuleResponse.Convert(modules);

        return new ApiResponse<List<ModuleResponse>>(mapped);
    }

    public async Task<ApiResponse<List<ModuleResponse>>> GetModulesToOrderAsync(IEnumerable<Guid> request, CancellationToken cancellationToken)
    {
        var enumerable = request as Guid[] ?? [.. request];
        var modules = await moduleRepository.GetManyOrdersAsync(enumerable, cancellationToken);
        var response = ModuleResponse.Convert(modules);

        return new ApiResponse<List<ModuleResponse>>(response);
    }

    public async Task<ApiResponse<ModuleResponse>> GetModuleByTypeAsync(ModuleType moduleType, CancellationToken cancellationToken)
    {
        var module = await moduleRepository.GetModuleByTypeAsync(moduleType, cancellationToken);

        if (module is null)
        {
            return new ApiResponse<ModuleResponse>(data: null, HttpStatusCode.NotFound, TextConstants.ItemNotFoundMessage);
        }

        var response = ModuleResponse.Convert(module);

        return new ApiResponse<ModuleResponse>(response);
    }

    public async Task<ApiResponse<int>> CountAsync(CancellationToken cancellationToken)
    {
        var response = await moduleRepository.CountAsync(cancellationToken);

        return new ApiResponse<int>(response);
    }

    public async Task<ApiResponse<List<ModuleResponse>>> LoadModulesAtDatabaseAsync(CancellationToken cancellationToken)
    {
        var modulesToSave = new List<ModuleModel>
                            {
                                new () { Name = "ERP", Amount = -1, Type = ModuleType.Erp },
                                new () { Name = "Auth", Amount = 10, Type = ModuleType.Auth },
                                new () { Name = "Basic", Amount = 20, Type = ModuleType.Basic },
                                new () { Name = "Social Network", Amount = 20, Type = ModuleType.SocialNetwork },
                                new () { Name = "Project", Amount = 20, Type = ModuleType.Project },
                                new () { Name = "Performance Evaluation", Amount = 20, Type = ModuleType.PerformanceEvaluation },
                                new () { Name = "Accounting", Amount = 20, Type = ModuleType.Accounting },
                                new () { Name = "HR", Amount = 20, Type = ModuleType.Hr },
                                new () { Name = "POS", Amount = 20, Type = ModuleType.Pos },
                                new () { Name = "Contracts", Amount = 20, Type = ModuleType.Contracts },
                                new () { Name = "Ecommerce", Amount = 20, Type = ModuleType.Ecommerce },
                                new () { Name = "Customer Support", Amount = 20, Type = ModuleType.CustomerSupport },
                                new () { Name = "Plus", Amount = 20, Type = ModuleType.Plus }
                            };

        var response = await moduleRepository.LoadModulesAtDatabaseAsync(modulesToSave, cancellationToken);
        var mapped = ModuleResponse.Convert(response);

        return new ApiResponse<List<ModuleResponse>>(mapped);
    }

    public async Task<ApiResponse<List<ModuleResponse>>> GetUserModulesAsync(Guid userId, Guid companyId, CancellationToken cancellationToken)
    {
        var userModules = await moduleRepository.GetUserModulesAsync(userId, companyId, cancellationToken);
        var mapped = ModuleResponse.Convert(userModules);

        return new ApiResponse<List<ModuleResponse>>(mapped);
    }

    public async Task<ApiResponse<List<ModuleResponse>>> GetModuleAndSubmoduleAsync(Guid userId, Guid companyId, CancellationToken cancellationToken)
    {
        var modules = await moduleRepository.GetModuleAndSubmoduleAsync(userId, companyId, cancellationToken);
        var mapped = ModuleResponse.Convert(modules);

        return new ApiResponse<List<ModuleResponse>>(mapped);
    }
}
