using Fenicia.Common.Data.Abstracts;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Auth;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Auth.Domains.Submodule;

public class SubmoduleRepository(AuthContext context) : BaseRepository<SubmoduleModel>(context), ISubmoduleRepository
{
    public async Task<List<SubmoduleModel>> GetByModuleIdAsync(Guid moduleId, CancellationToken ct)
    {
        return await context.Submodules.Where(sm => sm.ModuleId == moduleId).ToListAsync(ct);
    }
}
