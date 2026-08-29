using Fenicia.Auth.Domains.Module.DTOs;
using Fenicia.Common;

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
}
