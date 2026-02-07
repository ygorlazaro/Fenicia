using System.ComponentModel.DataAnnotations;

using Fenicia.Common.Data.Abstracts;
using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.Enums.Auth;

namespace Fenicia.Auth.Domains.Module;

public interface IModuleRepository : IBaseRepository<ModuleModel>
{
    Task<List<ModuleModel>> GetManyOrdersAsync([Required] IEnumerable<Guid> request, CancellationToken ct);

    Task<ModuleModel?> GetModuleByTypeAsync(ModuleType moduleType, CancellationToken ct);

    Task<List<ModuleModel>> LoadModulesAtDatabaseAsync(List<ModuleModel> modules, CancellationToken ct);

    Task<List<ModuleModel>> GetUserModulesAsync(Guid userId, Guid companyId, CancellationToken ct);

    Task<List<ModuleModel>> GetModuleAndSubmoduleAsync(Guid userId, Guid companyId, CancellationToken ct);
}