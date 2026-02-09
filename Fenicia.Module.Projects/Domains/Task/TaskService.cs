using Fenicia.Common.Data.Models.Project;
using Fenicia.Common.Data.Responses.Project;

namespace Fenicia.Module.Projects.Domains.Task;

public class TaskService(ITaskRepository taskRepository) : ITaskService
{
    public async Task<TaskResponse?> AddAsync(TaskRequest request, Guid userId, CancellationToken ct)
    {
        var task = new TaskModel(request)
        {
            CreatedBy = userId
        };

        taskRepository.Add(task);
        await taskRepository.SaveChangesAsync(ct);
        
        return new TaskResponse(task);
    }
}