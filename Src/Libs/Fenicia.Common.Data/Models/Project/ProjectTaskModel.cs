using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.Enums.Project;

namespace Fenicia.Common.Data.Models.Project;

[Table("tasks", Schema = "project")]
public class ProjectTaskModel : BaseCompanyModel
{
    public Guid ProjectId { get; set; }

    public Guid StatusId { get; set; }

    [MaxLength(256)]
    public string Title { get; set; } = null!;

    [MaxLength(4096)]
    public string? Description { get; set; } = null;

    public EnumTaskPriority Priority { get; set; } = EnumTaskPriority.Medium;

    public EnumTaskType Type { get; set; } = EnumTaskType.Task;

    public int Order { get; set; } = 0;

    public int? EstimatePoints { get; set; } = null;

    public DateTime? DueDate { get; set; } = null;

    public Guid CreatedBy { get; set; } = Guid.Empty;

    public virtual List<AttachmentModel> Attachments { get; set; } = [];

    public virtual List<ProjectCommentModel> Comments { get; set; } = [];

    public virtual List<ProjectSubtaskModel> Subtasks { get; set; } = [];

    public virtual List<TaskAssigneeModel> Assignees { get; set; } = [];

    public virtual ProjectStatusModel StatusModel { get; set; } = null!;

    public virtual UserModel User { get; set; } = null!;

    public virtual ProjectModel ProjectModel { get; set; } = null!;
}
