using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.Data.Repositories;
using Fenicia.Common.Enums.Auth;

namespace Fenicia.Auth.Domains.Module.Interfaces;

public interface IModuleRepository : IRepository<ModuleModel>
{
    Task<List<ModuleModel>> GetAllActiveAsync(int page, int perPage, CancellationToken cancellationToken = default);

    Task<int> CountAllActiveAsync(CancellationToken cancellationToken = default);

    Task<List<ModuleModel>> GetByIdsAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken = default);

    Task<ModuleModel?> GetByTypeAsync(ModuleType type, CancellationToken cancellationToken = default);
}
