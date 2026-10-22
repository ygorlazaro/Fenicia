using Fenicia.Common.Data.Models.Project;
using Fenicia.Common.Data.Repositories;
using Fenicia.Module.Projects.Domains.ProjectSubtask.Interfaces;
using Microsoft.EntityFrameworkCore;
using Fenicia.Common;

/// <summary>
/// Fenicia.Module.Projects/Domains/ProjectSubtask/ProjectSubtaskRepository.cs implementation.
/// </summary>

namespace Fenicia.Module.Projects.Domains.ProjectSubtask;

public class ProjectSubtaskRepository(DbContext context)
    : Repository<ProjectSubtaskModel>(context), IProjectSubtaskRepository
{
    public IQueryable<ProjectSubtaskModel> Query()
    {
        return base.Query();
    }

    public Task<ProjectSubtaskModel?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return base.GetByIdAsync(id, cancellationToken);
    }

    public Task<ProjectSubtaskModel> InsertAsync(ProjectSubtaskModel model, CancellationToken cancellationToken = default)
    {
        return base.InsertAsync(model, cancellationToken);
    }

    public Task<ProjectSubtaskModel?> UpdateAsync(Guid id, ProjectSubtaskModel model, CancellationToken cancellationToken = default)
    {
        return base.UpdateAsync(id, model, cancellationToken);
    }

    public Task<int> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return base.DeleteAsync(id, cancellationToken);
    }

    public Task<IEnumerable<ProjectSubtaskModel>> GetAllAsync(PaginationQuery query, CancellationToken cancellationToken = default)
    {
        return base.GetAllAsync(page, perPage, cancellationToken);
    }
}