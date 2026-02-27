using System.ComponentModel.DataAnnotations.Schema;

using Fenicia.Common.Enums.Project;

namespace Fenicia.Common.Data.Models;

[Table("task_assignees", Schema = "project")]
public class ProjectTaskAssignee: BaseModel
{
    public Guid TaskId { get; set; } = Guid.Empty;

    public Guid UserId { get; set; } = Guid.Empty;

    public AssigneeRole Role { get; set; } = AssigneeRole.Owner;

    public DateTime AssignedAt { get; set; } = DateTime.UtcNow;

    public virtual ProjectTask Task { get; set; } = null!;
}
