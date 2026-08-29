using Fenicia.Auth.Domains.Module.DTOs;
using Fenicia.Common;
using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.Enums.Auth;

namespace Fenicia.Auth.Domains.Module;

public class ModuleService(ModuleRepository repository)
{
    public async Task<Pagination<List<GetModuleResponse>>> GetAllModulesAsync(int page, int perPage, CancellationToken ct)
    {
        var modules = await repository.GetAllActiveAsync(page, perPage, ct);
        var total = await repository.CountAllActiveAsync(ct);

        return new Pagination<List<GetModuleResponse>>(modules.Select(m => m.MapToGetModuleResponse()).ToList(), total, page, perPage);
    }

    public async Task<List<GetUserModulesResponse>> GetUserModulesAsync(Guid companyId, Guid userId, CancellationToken ct)
    {
        var modules = await repository.GetUserModulesAsync(companyId, userId, ct);

        return modules.Select(m => m.MapToGetUserModulesResponse()).ToList();
    }

    public async Task<List<ModuleModel>> GetModulesByIdsAsync(IEnumerable<Guid> ids, CancellationToken ct)
    {
        return await repository.GetByIdsAsync(ids, ct);
    }

    public async Task<ModuleModel?> GetModuleByTypeAsync(ModuleType type, CancellationToken ct)
    {
        return await repository.GetByTypeAsync(type, ct);
    }
}
