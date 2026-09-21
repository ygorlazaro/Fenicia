using Fenicia.Common.Data.Models.Project;

namespace Fenicia.Module.Projects.Domains.ProjectTaskAssignee.Interfaces;

public interface IProjectTaskAssigneeRepository
{
    IQueryable<TaskAssigneeModel> Query();

    Task<TaskAssigneeModel?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<TaskAssigneeModel> InsertAsync(TaskAssigneeModel model, CancellationToken cancellationToken = default);

    Task<TaskAssigneeModel?> UpdateAsync(Guid id, TaskAssigneeModel model, CancellationToken cancellationToken = default);

    Task<int> DeleteAsync(Guid id, CancellationToken cancellationToken = default);

    Task<IEnumerable<TaskAssigneeModel>> GetAllAsync(int page, int perPage, CancellationToken cancellationToken = default);
}