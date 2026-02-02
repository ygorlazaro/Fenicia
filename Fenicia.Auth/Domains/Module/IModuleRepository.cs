using System.ComponentModel.DataAnnotations;

using Fenicia.Common.Database.Models.Auth;
using Fenicia.Common.Enums;

namespace Fenicia.Auth.Domains.Module;

public interface IModuleRepository
{
    Task<List<ModuleModel>> GetAllOrderedAsync(CancellationToken cancellationToken, [Range(minimum: 1, int.MaxValue)] int page = 1, [Range(minimum: 1, maximum: 100)] int perPage = 10);

    Task<List<ModuleModel>> GetManyOrdersAsync([Required] IEnumerable<Guid> request, CancellationToken cancellationToken);

    Task<ModuleModel?> GetModuleByTypeAsync(ModuleType moduleType, CancellationToken cancellationToken);

    Task<int> CountAsync(CancellationToken cancellationToken);

    Task<List<ModuleModel>> LoadModulesAtDatabaseAsync(List<ModuleModel> modules, CancellationToken cancellationToken);

    Task<List<ModuleModel>> GetUserModulesAsync(Guid userId, Guid companyId, CancellationToken cancellationToken);

    Task<List<ModuleModel>> GetModuleAndSubmoduleAsync(Guid userId, Guid companyId, CancellationToken cancellationToken);
}
