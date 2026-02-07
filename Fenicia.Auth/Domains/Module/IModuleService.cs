using Fenicia.Common.Data.Responses.Auth;
using Fenicia.Common.Enums.Auth;

namespace Fenicia.Auth.Domains.Module;

public interface IModuleService
{
    Task<List<ModuleResponse>> GetAllOrderedAsync(CancellationToken ct, int page = 1, int perPage = 10);

    Task<List<ModuleResponse>> GetModulesToOrderAsync(IEnumerable<Guid> request, CancellationToken ct);

    Task<ModuleResponse?> GetModuleByTypeAsync(ModuleType moduleType, CancellationToken ct);

    Task<int> CountAsync(CancellationToken ct);

    Task<List<ModuleResponse>> LoadModulesAtDatabaseAsync(CancellationToken ct);

    Task<List<ModuleResponse>> GetUserModulesAsync(Guid user, Guid companyId, CancellationToken ct);

    Task<List<ModuleResponse>> GetModuleAndSubmoduleAsync(Guid userId, Guid companyId, CancellationToken ct);
}