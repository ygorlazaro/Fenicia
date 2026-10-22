using Fenicia.Common.Data.Models.Project;
using Fenicia.Common.Data.Repositories;
using Fenicia.Module.Projects.Domains.Project.Interfaces;
using Microsoft.EntityFrameworkCore;
using Fenicia.Common;

/// <summary>
/// Fenicia.Module.Projects/Domains/Project/ProjectRepository.cs implementation.
/// </summary>

namespace Fenicia.Module.Projects.Domains.Project;

public class ProjectRepository(DbContext context) : Repository<ProjectModel>(context), IProjectRepository
{
    public override IQueryable<ProjectModel> Query()
    {
        return base.Query();
    }

    public override Task<ProjectModel?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
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

    public override Task<ProjectModel> InsertAsync(ProjectModel model, CancellationToken cancellationToken = default)
    {
        return base.InsertAsync(model, cancellationToken);
    }

    public override Task<ProjectModel?> UpdateAsync(Guid id, ProjectModel model, CancellationToken cancellationToken = default)
    {
        return base.UpdateAsync(id, model, cancellationToken);
    }

    public override Task<int> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return base.DeleteAsync(id, cancellationToken);
    }

    public override Task<IEnumerable<ProjectModel>> GetAllAsync(PaginationQuery query, CancellationToken cancellationToken = default)
    {
        return base.GetAllAsync(query, cancellationToken);
    }
}