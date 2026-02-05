using Fenicia.Common.Database.Models.Auth;
using Fenicia.Common.Database.Responses;
using Fenicia.Common.Enums;

namespace Fenicia.Auth.Domains.Module;

public class ModuleService(IModuleRepository moduleRepository) : IModuleService
{
    public async Task<List<ModuleResponse>> GetAllOrderedAsync(CancellationToken cancellationToken, int page = 1, int perPage = 10)
    {
        var modules = await moduleRepository.GetAllAsync(cancellationToken, page, perPage);

        return ModuleResponse.Convert(modules);
    }

    public async Task<List<ModuleResponse>> GetModulesToOrderAsync(IEnumerable<Guid> request, CancellationToken cancellationToken)
    {
        var enumerable = request as Guid[] ?? [.. request];
        var modules = await moduleRepository.GetManyOrdersAsync(enumerable, cancellationToken);
        var response = ModuleResponse.Convert(modules);

        return response;
    }

    public async Task<ModuleResponse?> GetModuleByTypeAsync(ModuleType moduleType, CancellationToken cancellationToken)
    {
        var module = await moduleRepository.GetModuleByTypeAsync(moduleType, cancellationToken);

        return module is null ? null : ModuleResponse.Convert(module);
    }

    public async Task<int> CountAsync(CancellationToken cancellationToken)
    {
        return await moduleRepository.CountAsync(cancellationToken);
    }

    public async Task<List<ModuleResponse>> LoadModulesAtDatabaseAsync(CancellationToken cancellationToken)
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

        return ModuleResponse.Convert(response);
    }

    public async Task<List<ModuleResponse>> GetUserModulesAsync(Guid userId, Guid companyId, CancellationToken cancellationToken)
    {
        var userModules = await moduleRepository.GetUserModulesAsync(userId, companyId, cancellationToken);

        return ModuleResponse.Convert(userModules);
    }

    public async Task<List<ModuleResponse>> GetModuleAndSubmoduleAsync(Guid userId, Guid companyId, CancellationToken cancellationToken)
    {
        var modules = await moduleRepository.GetModuleAndSubmoduleAsync(userId, companyId, cancellationToken);

        return ModuleResponse.Convert(modules);
    }
}
