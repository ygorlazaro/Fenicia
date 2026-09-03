using System.ComponentModel.DataAnnotations.Schema;
using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.Enums.Project;

namespace Fenicia.Common.Data.Models.Project;

[Table("task_assignees", Schema = "project")]
public sealed class TaskAssigneeModel : BaseCompanyModel
{
    public Guid TaskId { get; init; } = Guid.Empty;

    public Guid UserId { get; init; } = Guid.Empty;

    public EnumAssigneeRole Role { get; init; } = EnumAssigneeRole.Owner;

    public DateTime AssignedAt { get; init; } = DateTime.UtcNow;

    public UserModel User { get; init; } = default!;

    public ProjectTaskModel TaskModel { get; init; } = default!;
}