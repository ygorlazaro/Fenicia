using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Project;
using Fenicia.Common.Data.Repositories;
using Fenicia.Module.Projects.Domains.Project.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Projects.Domains.Project;

public class ProjectRepository(DefaultContext context) : Repository<ProjectModel>(context), IProjectRepository
{
    public async Task<ProjectModel?> GetByIdWithRelationsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await DbSet
                .Include(p => p.Statuses)
            .Include(p => p.Tasks)
            .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
    }
}
