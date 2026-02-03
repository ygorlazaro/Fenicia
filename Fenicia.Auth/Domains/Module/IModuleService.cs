using Fenicia.Common.Database.Responses;
using Fenicia.Common.Enums;

namespace Fenicia.Auth.Domains.Module;

public interface IModuleService
{
    Task<List<ModuleResponse>> GetAllOrderedAsync(CancellationToken cancellationToken, int page = 1, int perPage = 10);

    Task<List<ModuleResponse>> GetModulesToOrderAsync(IEnumerable<Guid> request, CancellationToken cancellationToken);

    Task<ModuleResponse?> GetModuleByTypeAsync(ModuleType moduleType, CancellationToken cancellationToken);

    Task<int> CountAsync(CancellationToken cancellationToken);

    Task<List<ModuleResponse>> LoadModulesAtDatabaseAsync(CancellationToken cancellationToken);

    Task<List<ModuleResponse>> GetUserModulesAsync(Guid user, Guid companyId, CancellationToken cancellationToken);

    Task<List<ModuleResponse>> GetModuleAndSubmoduleAsync(Guid userId, Guid companyId, CancellationToken cancellationToken);
}
