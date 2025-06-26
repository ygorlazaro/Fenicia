using Fenicia.Auth.Domains.Module.Data;
using Fenicia.Common;
using Fenicia.Common.Enums;

namespace Fenicia.Auth.Domains.Module.Logic;

public interface IModuleService
{
    Task<ApiResponse<List<ModuleResponse>>> GetAllOrderedAsync(CancellationToken cancellationToken,
        int page = 1, int perPage = 10);
    Task<ApiResponse<List<ModuleResponse>>> GetModulesToOrderAsync(IEnumerable<Guid> request,
        CancellationToken cancellationToken);
    Task<ApiResponse<ModuleResponse>> GetModuleByTypeAsync(ModuleType moduleType,
        CancellationToken cancellationToken);
    Task<ApiResponse<int>> CountAsync(CancellationToken cancellationToken);
}
