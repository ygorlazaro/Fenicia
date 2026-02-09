using Fenicia.Common.Data.Requests.Project;
using Fenicia.Common.Enums.Project;

namespace Fenicia.Common.Data.Models.Project;

public class TaskAssigneeModel(TaskAssigneeRequest request) : BaseModel
{
    public Guid TaskId { get; set; } = request.TaskId;

    public Guid UserId { get; set; } = request.UserId;

    public AssigneeRole Role { get; set; } = request.Role;

    public DateTime AssignedAt { get; set; } = request.AssignedAt;

    public virtual TaskModel Task { get; set; } = null!;
}