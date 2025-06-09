using Fenicia.Auth.Contexts.Models;
using Fenicia.Common.Enums;

namespace Fenicia.Auth.Repositories.Interfaces;

public interface IModuleRepository
{
    Task<List<ModuleModel>> GetAllOrderedAsync(int page = 1, int perPage = 10);
    Task<List<ModuleModel>> GetManyOrdersAsync(IEnumerable<Guid> request);
    Task<ModuleModel?> GetModuleByTypeAsync(ModuleType moduleType);
    Task<int> CountAsync();
}
