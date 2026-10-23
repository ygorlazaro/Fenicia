using Fenicia.Common;
using Fenicia.Common.Data.Models.Project;
using Fenicia.Common.Data.Repositories;
using Fenicia.Module.Projects.Domains.ProjectTask.Interfaces;
using Microsoft.EntityFrameworkCore;

/// <summary>
/// Fenicia.Module.Projects/Domains/ProjectTask/ProjectTaskRepository.cs implementation.
/// </summary>

namespace Fenicia.Module.Projects.Domains.ProjectTask;

public class ProjectTaskRepository(DbContext context) : Repository<ProjectTaskModel>(context), IProjectTaskRepository
{
    public override IQueryable<ProjectTaskModel> Query()
    {
        return base.Query();
    }

    public override Task<ProjectTaskModel?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
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

    public override Task<ProjectTaskModel> InsertAsync(ProjectTaskModel model, CancellationToken cancellationToken = default)
    {
        return base.InsertAsync(model, cancellationToken);
    }

    public override Task<ProjectTaskModel?> UpdateAsync(Guid id, ProjectTaskModel model, CancellationToken cancellationToken = default)
    {
        return base.UpdateAsync(id, model, cancellationToken);
    }

    public override Task<int> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return base.DeleteAsync(id, cancellationToken);
    }

    public override Task<IEnumerable<ProjectTaskModel>> GetAllAsync(PaginationQuery query, CancellationToken cancellationToken = default)
    {
        return base.GetAllAsync(query, cancellationToken);
    }
}