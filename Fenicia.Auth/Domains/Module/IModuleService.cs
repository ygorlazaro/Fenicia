using Fenicia.Common;
using Fenicia.Common.Database.Responses;
using Fenicia.Common.Enums;

namespace Fenicia.Auth.Domains.Module;

public interface IModuleService
{
    Task<ApiResponse<List<ModuleResponse>>> GetAllOrderedAsync(CancellationToken cancellationToken, int page = 1, int perPage = 10);

    Task<ApiResponse<List<ModuleResponse>>> GetModulesToOrderAsync(IEnumerable<Guid> request, CancellationToken cancellationToken);

    Task<ApiResponse<ModuleResponse>> GetModuleByTypeAsync(ModuleType moduleType, CancellationToken cancellationToken);

    Task<ApiResponse<int>> CountAsync(CancellationToken cancellationToken);

    Task<ApiResponse<List<ModuleResponse>>> LoadModulesAtDatabaseAsync(CancellationToken cancellationToken);

    Task<ApiResponse<List<ModuleResponse>>> GetUserModulesAsync(Guid user, Guid companyId, CancellationToken cancellationToken);

    Task<ApiResponse<List<ModuleResponse>>> GetModuleAndSubmoduleAsync(Guid userId, Guid companyId, CancellationToken cancellationToken);
}
