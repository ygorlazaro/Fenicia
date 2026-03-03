using System.ComponentModel.DataAnnotations.Schema;

using Fenicia.Common.Enums.Project;

namespace Fenicia.Common.Data.Models;

[Table("task_assignees", Schema = "project")]
public class ProjectTaskAssigneeModel : BaseCompanyModel
{
    public Guid TaskId { get; set; } = Guid.Empty;

    public Guid UserId { get; set; } = Guid.Empty;

    public AssigneeRole Role { get; set; } = AssigneeRole.Owner;

    public DateTime AssignedAt { get; set; } = DateTime.UtcNow;
    
    public virtual AuthUserModel UserModel { get; set; }

    public virtual ProjectTaskModel TaskModel { get; set; } = null!;
}
