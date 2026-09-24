using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.Data.Repositories;
using Fenicia.Common.Enums.Auth;

namespace Fenicia.Auth.Domains.Module.Interfaces;

public interface IModuleRepository : IRepository<ModuleModel>
{
    Task<List<ModuleModel>> GetByIdsAsync(
        IEnumerable<Guid> ids,
        CancellationToken cancellationToken = default);

    Task<ModuleModel?> GetByTypeAsync(EnumModuleType type, CancellationToken cancellationToken = default);

    Task<List<ModuleModel>> GetActiveModulesAsync(CancellationToken cancellationToken = default);

    Task<int> GetTotalActiveModulesAsync(CancellationToken cancellationToken = default);
}
