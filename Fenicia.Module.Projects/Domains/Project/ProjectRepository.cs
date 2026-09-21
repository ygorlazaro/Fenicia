using Fenicia.Common.Data.Models.Project;
using Fenicia.Common.Data.Repositories;
using Fenicia.Module.Projects.Domains.Project.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Projects.Domains.Project;

public class ProjectRepository(DbContext context) : Repository<ProjectModel>(context), IProjectRepository
{
    public new IQueryable<ProjectModel> Query()
    {
        return base.Query();
    }

    public new Task<ProjectModel?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return base.GetByIdAsync(id, cancellationToken);
    }

    public Task<ProjectModel?> GetByIdWithRelationsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return DbSet
            .Include(p => p.Statuses)
            .Include(p => p.Tasks)
            .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
    }

    public new Task<ProjectModel> InsertAsync(ProjectModel model, CancellationToken cancellationToken = default)
    {
        return base.InsertAsync(model, cancellationToken);
    }

    public new Task<ProjectModel?> UpdateAsync(Guid id, ProjectModel model, CancellationToken cancellationToken = default)
    {
        return base.UpdateAsync(id, model, cancellationToken);
    }

    public new Task<int> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return base.DeleteAsync(id, cancellationToken);
    }

    public new Task<IEnumerable<ProjectModel>> GetAllAsync(int page, int perPage, CancellationToken cancellationToken = default)
    {
        return base.GetAllAsync(page, perPage, cancellationToken);
    }
}