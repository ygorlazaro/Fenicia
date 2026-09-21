using Fenicia.Common.Data.Models.Project;
using Fenicia.Common.Data.Repositories;
using Fenicia.Module.Projects.Domains.ProjectTask.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Projects.Domains.ProjectTask;

public class ProjectTaskRepository(DbContext context) : Repository<ProjectTaskModel>(context), IProjectTaskRepository
{
    public new IQueryable<ProjectTaskModel> Query()
    {
        return base.Query();
    }

    public new Task<ProjectTaskModel?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return base.GetByIdAsync(id, cancellationToken);
    }

    public Task<ProjectTaskModel?> GetByIdWithRelationsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return DbSet
            .Include(pt => pt.Attachments)
            .Include(pt => pt.Comments)
            .Include(pt => pt.Subtasks)
            .Include(pt => pt.Assignees)
            .ThenInclude(a => a.User)
            .Include(pt => pt.SprintModel)
            .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
    }

    public new Task<ProjectTaskModel> InsertAsync(ProjectTaskModel model, CancellationToken cancellationToken = default)
    {
        return base.InsertAsync(model, cancellationToken);
    }

    public new Task<ProjectTaskModel?> UpdateAsync(Guid id, ProjectTaskModel model, CancellationToken cancellationToken = default)
    {
        return base.UpdateAsync(id, model, cancellationToken);
    }

    public new Task<int> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return base.DeleteAsync(id, cancellationToken);
    }

    public new Task<IEnumerable<ProjectTaskModel>> GetAllAsync(int page, int perPage, CancellationToken cancellationToken = default)
    {
        return base.GetAllAsync(page, perPage, cancellationToken);
    }
}