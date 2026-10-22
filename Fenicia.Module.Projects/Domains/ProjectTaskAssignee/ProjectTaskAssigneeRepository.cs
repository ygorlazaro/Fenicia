using Fenicia.Common.Data.Models.Project;
using Fenicia.Common.Data.Repositories;
using Fenicia.Module.Projects.Domains.ProjectTaskAssignee.Interfaces;
using Microsoft.EntityFrameworkCore;
using Fenicia.Common;

/// <summary>
/// Fenicia.Module.Projects/Domains/ProjectTaskAssignee/ProjectTaskAssigneeRepository.cs implementation.
/// </summary>

namespace Fenicia.Module.Projects.Domains.ProjectTaskAssignee;

public class ProjectTaskAssigneeRepository(DbContext context) : Repository<TaskAssigneeModel>(context), IProjectTaskAssigneeRepository
{
    public override IQueryable<TaskAssigneeModel> Query()
    {
        return base.Query();
    }

    public override Task<TaskAssigneeModel?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return base.GetByIdAsync(id, cancellationToken);
    }

    public override Task<TaskAssigneeModel> InsertAsync(TaskAssigneeModel model, CancellationToken cancellationToken = default)
    {
        return base.InsertAsync(model, cancellationToken);
    }

    public override Task<TaskAssigneeModel?> UpdateAsync(Guid id, TaskAssigneeModel model, CancellationToken cancellationToken = default)
    {
        return base.UpdateAsync(id, model, cancellationToken);
    }

    public override Task<int> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return base.DeleteAsync(id, cancellationToken);
    }

    public override Task<IEnumerable<TaskAssigneeModel>> GetAllAsync(PaginationQuery query, CancellationToken cancellationToken = default)
    {
        return base.GetAllAsync(page, perPage, cancellationToken);
    }
}