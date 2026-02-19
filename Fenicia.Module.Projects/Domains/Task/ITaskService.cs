using Fenicia.Common.Data.Requests.Project;
using Fenicia.Common.Data.Responses.Project;

namespace Fenicia.Module.Projects.Domains.Task;

public interface ITaskService
{
    Task<TaskResponse?> AddAsync(TaskRequest request, Guid userId, CancellationToken ct);
}