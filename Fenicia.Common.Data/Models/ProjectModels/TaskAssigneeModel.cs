using System.ComponentModel.DataAnnotations.Schema;

using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.Enums.Project;

namespace Fenicia.Common.Data.Models.ProjectModels;

[Table("task_assignees", Schema = "project")]
public class TaskAssigneeModel : BaseCompanyModel
{
    public Guid TaskId { get; set; } = Guid.Empty;

    public Guid UserId { get; set; } = Guid.Empty;

    public EnumAssigneeRole Role { get; set; } = EnumAssigneeRole.Owner;

    public DateTime AssignedAt { get; set; } = DateTime.UtcNow;

    public virtual UserModel User { get; set; } = null!;

    public virtual ProjectTaskModel TaskModel { get; set; } = null!;
}
