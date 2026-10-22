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
    public override IQueryable<ProjectSubtaskModel> Query()
    {
        return base.Query();
    }

    public override Task<ProjectSubtaskModel?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return base.GetByIdAsync(id, cancellationToken);
    }

    public override Task<ProjectSubtaskModel> InsertAsync(ProjectSubtaskModel model, CancellationToken cancellationToken = default)
    {
        return base.InsertAsync(model, cancellationToken);
    }

    public override Task<ProjectSubtaskModel?> UpdateAsync(Guid id, ProjectSubtaskModel model, CancellationToken cancellationToken = default)
    {
        return base.UpdateAsync(id, model, cancellationToken);
    }

    public override Task<int> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return base.DeleteAsync(id, cancellationToken);
    }

    public override Task<IEnumerable<ProjectSubtaskModel>> GetAllAsync(PaginationQuery query, CancellationToken cancellationToken = default)
    {
        return base.GetAllAsync(page, perPage, cancellationToken);
    }
}