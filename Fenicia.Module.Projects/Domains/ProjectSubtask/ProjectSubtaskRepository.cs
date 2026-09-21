using Fenicia.Common.Data.Models.Project;
using Fenicia.Common.Data.Repositories;
using Fenicia.Module.Projects.Domains.ProjectSubtask.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Projects.Domains.ProjectSubtask;

public class ProjectSubtaskRepository(DbContext context)
    : Repository<ProjectSubtaskModel>(context), IProjectSubtaskRepository
{
    public new IQueryable<ProjectSubtaskModel> Query()
    {
        return base.Query();
    }

    public new Task<ProjectSubtaskModel?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return base.GetByIdAsync(id, cancellationToken);
    }

    public new Task<ProjectSubtaskModel> InsertAsync(ProjectSubtaskModel model, CancellationToken cancellationToken = default)
    {
        return base.InsertAsync(model, cancellationToken);
    }

    public new Task<ProjectSubtaskModel?> UpdateAsync(Guid id, ProjectSubtaskModel model, CancellationToken cancellationToken = default)
    {
        return base.UpdateAsync(id, model, cancellationToken);
    }

    public new Task<int> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return base.DeleteAsync(id, cancellationToken);
    }

    public new Task<IEnumerable<ProjectSubtaskModel>> GetAllAsync(int page, int perPage, CancellationToken cancellationToken = default)
    {
        return base.GetAllAsync(page, perPage, cancellationToken);
    }
}