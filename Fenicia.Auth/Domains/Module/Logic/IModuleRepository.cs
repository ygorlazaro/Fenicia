namespace Fenicia.Auth.Domains.Module.Logic;

using System.ComponentModel.DataAnnotations;

using Common.Enums;

using Data;

public interface IModuleRepository
{
    Task<List<ModuleModel>> GetAllOrderedAsync(CancellationToken cancellationToken, [Range(minimum: 1, int.MaxValue)] int page = 1, [Range(minimum: 1, maximum: 100)] int perPage = 10);

    Task<List<ModuleModel>> GetManyOrdersAsync([Required] IEnumerable<Guid> request, CancellationToken cancellationToken);

    Task<ModuleModel?> GetModuleByTypeAsync(ModuleType moduleType, CancellationToken cancellationToken);

    Task<int> CountAsync(CancellationToken cancellationToken);

    Task<List<ModuleModel>> LoadModulesAtDatabaseAsync(List<ModuleModel> modules, CancellationToken cancellationToken);
}
