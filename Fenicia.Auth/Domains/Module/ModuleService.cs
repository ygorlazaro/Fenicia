using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.Data.Responses.Auth;
using Fenicia.Common.Enums.Auth;

namespace Fenicia.Auth.Domains.Module;

public class ModuleService(IModuleRepository moduleRepository) : IModuleService
{
    public async Task<List<ModuleResponse>> GetAllOrderedAsync(CancellationToken ct, int page = 1, int perPage = 10)
    {
        var modules = await moduleRepository.GetAllAsync(ct, page, perPage);

        return [.. modules.Select(module => new ModuleResponse(module))];
    }

    public async Task<List<ModuleResponse>> GetModulesToOrderAsync(IEnumerable<Guid> request, CancellationToken ct)
    {
        var enumerable = request as Guid[] ?? [.. request];
        var modules = await moduleRepository.GetManyOrdersAsync(enumerable, ct);
        var response = modules.Select(module => new ModuleResponse(module)).ToList();

        return response;
    }

    public async Task<ModuleResponse?> GetModuleByTypeAsync(ModuleType moduleType, CancellationToken ct)
    {
        var module = await moduleRepository.GetModuleByTypeAsync(moduleType, ct);

        return module is null ? null : new ModuleResponse(module);
    }

    public async Task<int> CountAsync(CancellationToken ct)
    {
        return await moduleRepository.CountAsync(ct);
    }

    public async Task<List<ModuleResponse>> LoadModulesAtDatabaseAsync(CancellationToken ct)
    {
        var modulesToSave = new List<ModuleModel>
        {
            new() { Name = "ERP", Price = -1, Type = ModuleType.Erp },
            new() { Name = "Auth", Price = 10, Type = ModuleType.Auth },
            new() { Name = "Basic", Price = 20, Type = ModuleType.Basic },
            new() { Name = "Social Network", Price = 20, Type = ModuleType.SocialNetwork },
            new() { Name = "Project", Price = 20, Type = ModuleType.Project },
            new() { Name = "Performance Evaluation", Price = 20, Type = ModuleType.PerformanceEvaluation },
            new() { Name = "Accounting", Price = 20, Type = ModuleType.Accounting },
            new() { Name = "HR", Price = 20, Type = ModuleType.Hr },
            new() { Name = "POS", Price = 20, Type = ModuleType.Pos },
            new() { Name = "Contracts", Price = 20, Type = ModuleType.Contracts },
            new() { Name = "Ecommerce", Price = 20, Type = ModuleType.Ecommerce },
            new() { Name = "Customer Support", Price = 20, Type = ModuleType.CustomerSupport },
            new() { Name = "Plus", Price = 20, Type = ModuleType.Plus }
        };

        var modules = await moduleRepository.LoadModulesAtDatabaseAsync(modulesToSave, ct);

        return [.. modules.Select(module => new ModuleResponse(module))];
    }

    public async Task<List<ModuleResponse>> GetUserModulesAsync(Guid userId, Guid companyId, CancellationToken ct)
    {
        var modules = await moduleRepository.GetUserModulesAsync(userId, companyId, ct);

        return [.. modules.Select(module => new ModuleResponse(module))];
    }

    public async Task<List<ModuleResponse>> GetModuleAndSubmoduleAsync(
        Guid userId,
        Guid companyId,
        CancellationToken ct)
    {
        var modules = await moduleRepository.GetModuleAndSubmoduleAsync(userId, companyId, ct);

        return [.. modules.Select(module => new ModuleResponse(module))];
    }
}
