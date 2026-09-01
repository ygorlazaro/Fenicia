using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Project;
using Fenicia.Common.Data.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Projects.Domains.ProjectTask;

public class ProjectTaskRepository(DefaultContext context) : Repository<ProjectTaskModel>(context)
{
    public async Task<ProjectTaskModel?> GetByIdWithRelationsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await DbSet
                .Include(pt => pt.Attachments)
            .Include(pt => pt.Comments)
            .Include(pt => pt.Subtasks)
            .Include(pt => pt.Assignees)
                .ThenInclude(a => a.User)
            .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
    }
}
