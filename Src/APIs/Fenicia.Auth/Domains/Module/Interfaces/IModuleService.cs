using Fenicia.Auth.Domains.Module.DTOs;
using Fenicia.Common;
using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.Enums.Auth;

namespace Fenicia.Auth.Domains.Module.Interfaces;

public interface IModuleService
{
    Task<Pagination<List<GetModuleResponse>>> GetAllModulesAsync(PaginationQuery query, CancellationToken cancellationToken = default);

    Task<List<GetUserModulesResponse>> GetUserModulesAsync(Guid companyId, Guid userId, CancellationToken cancellationToken = default);

    Task<List<ModuleModel>> GetModulesByIdsAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken = default);

    Task<ModuleModel?> GetModuleByTypeAsync(ModuleType type, CancellationToken cancellationToken = default);
}
