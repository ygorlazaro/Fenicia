using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.ProjectModels;
using Fenicia.Common.Data.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Projects.Domains.Project;

public class ProjectRepository(DefaultContext context) : Repository<ProjectModel>(context)
{
    public async Task<ProjectModel?> GetByIdWithRelationsAsync(Guid id, CancellationToken ct)
    {
        return await DbSet
                .Include(p => p.Statuses)
            .Include(p => p.Tasks)
            .FirstOrDefaultAsync(e => e.Id == id, ct);
    }
}
