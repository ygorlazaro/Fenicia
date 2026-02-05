using System.ComponentModel.DataAnnotations;

using Fenicia.Common.Data.Abstracts;
using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.Enums;

namespace Fenicia.Auth.Domains.Module;

public interface IModuleRepository : IBaseRepository<ModuleModel>
{
    Task<List<ModuleModel>> GetManyOrdersAsync([Required] IEnumerable<Guid> request, CancellationToken cancellationToken);

    Task<ModuleModel?> GetModuleByTypeAsync(ModuleType moduleType, CancellationToken cancellationToken);

    Task<List<ModuleModel>> LoadModulesAtDatabaseAsync(List<ModuleModel> modules, CancellationToken cancellationToken);

    Task<List<ModuleModel>> GetUserModulesAsync(Guid userId, Guid companyId, CancellationToken cancellationToken);

    Task<List<ModuleModel>> GetModuleAndSubmoduleAsync(Guid userId, Guid companyId, CancellationToken cancellationToken);
}
