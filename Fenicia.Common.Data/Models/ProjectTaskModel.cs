using System.ComponentModel.DataAnnotations.Schema;

using Fenicia.Common.Enums.Project;

namespace Fenicia.Common.Data.Models;

[Table("tasks", Schema = "project")]
public class ProjectTaskModel : BaseCompanyModel
{
    public Guid ProjectId { get; set; }

    public Guid StatusId { get; set; }

    public string Title { get; set; } = null!;

    public string? Description { get; set; } = null!;

    public TaskPriority Priority { get; set; } = TaskPriority.Medium;

    public TaskType Type { get; set; } = TaskType.Task;

    public int Order { get; set; } = 0;

    public int? EstimatePoints { get; set; } = null!;

    public DateTime? DueDate { get; set; } = null!;

    public Guid CreatedBy { get; set; } = Guid.Empty;

    public virtual List<ProjectAttachmentModel> Attachments { get; set; } = [];

    public virtual List<ProjectCommentModel> Comments { get; set; } = [];

    public virtual List<ProjectSubtaskModel> Subtasks { get; set; } = [];

    public virtual List<ProjectTaskAssigneeModel> Assignees { get; set; } = [];

    public virtual ProjectStatusModel StatusModel { get; set; } = null!;

    public virtual AuthUserModel UserModel { get; set; } = null!;

    public virtual ProjectModel ProjectModel { get; set; } = null!;
}
