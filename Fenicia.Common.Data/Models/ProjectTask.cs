using System.ComponentModel.DataAnnotations.Schema;

using Fenicia.Common.Enums.Project;

namespace Fenicia.Common.Data.Models;

[Table("tasks", Schema = "project")]
public class ProjectTask : BaseModel
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

    public virtual List<ProjectAttachment> Attachments { get; set; } = [];

    public virtual List<ProjectComment> Comments { get; set; } = [];

    public virtual List<ProjectSubtask> Subtasks { get; set; } = [];

    public virtual List<ProjectTaskAssignee> Assignees { get; set; } = [];

    public virtual ProjectStatus Status { get; set; } = null!;

    public virtual AuthUser User { get; set; } = null!;

    public virtual Project Project { get; set; } = null!;
}
