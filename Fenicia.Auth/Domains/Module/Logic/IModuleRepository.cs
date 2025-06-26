using Fenicia.Auth.Domains.Module.Data;
using Fenicia.Common.Enums;

namespace Fenicia.Auth.Domains.Module.Logic;

public interface IModuleRepository
{
    Task<List<ModuleModel>> GetAllOrderedAsync(CancellationToken cancellationToken, int page = 1,
        int perPage = 10);
    Task<List<ModuleModel>> GetManyOrdersAsync(IEnumerable<Guid> request,
        CancellationToken cancellationToken);
    Task<ModuleModel?> GetModuleByTypeAsync(ModuleType moduleType,
        CancellationToken cancellationToken);
    Task<int> CountAsync(CancellationToken cancellationToken);
}
