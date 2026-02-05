using Fenicia.Common.Database.Contexts;
using Fenicia.Common.Database.Models.Auth;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Auth.Domains.Submodule;

public class SubmoduleRepository(AuthContext context) : ISubmoduleRepository
{
    public async Task<List<SubmoduleModel>> GetByModuleIdAsync(Guid moduleId, CancellationToken cancellationToken)
    {
        return await context.Submodules.Where(sm => sm.ModuleId == moduleId).ToListAsync(cancellationToken);
    }
}
