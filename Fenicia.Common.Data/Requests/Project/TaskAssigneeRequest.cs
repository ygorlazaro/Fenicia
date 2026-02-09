using Fenicia.Common.Enums.Project;

namespace Fenicia.Common.Data.Requests.Project;

public class TaskAssigneeRequest
{
    public Guid TaskId { get; set; }

    public Guid UserId { get; set; }

    public AssigneeRole Role { get; set; }

    public DateTime AssignedAt { get; set; } = DateTime.UtcNow;
}