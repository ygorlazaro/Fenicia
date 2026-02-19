using Fenicia.Common.Data.Contexts;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Auth.Domains.Submodule.GetByModuleId;

public class GetByModuleIdHandler(AuthContext context)
{
    public async Task<List<GetByModuleResponse>> Handle(Guid moduleId, CancellationToken ct)
    {
        return await context.Submodules.Where(sm => sm.ModuleId == moduleId)
            .Select(sm => new GetByModuleResponse(sm.Id, sm.Name, sm.Description, sm.ModuleId, sm.Route))
            .ToListAsync(ct);
    }
}