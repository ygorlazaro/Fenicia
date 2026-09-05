using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.Enums.Project;

namespace Fenicia.Common.Data.Models.Project;

[Table("tasks", Schema = "project")]
public sealed class ProjectTaskModel : BaseCompanyModel
{
    public Guid ProjectId { get; init; }

    public Guid StatusId { get; init; }

    [MaxLength(256)]
    public string Title { get; init; } = string.Empty;

    [MaxLength(4096)]
    public string? Description { get; init; } = null;

    public EnumTaskPriority Priority { get; init; } = EnumTaskPriority.Medium;

    public EnumTaskType Type { get; init; } = EnumTaskType.Task;

    public int Order { get; init; } = 0;

    public int? EstimatePoints { get; init; } = null;

    public DateTime? DueDate { get; init; } = null;

    public Guid CreatedBy { get; init; } = Guid.Empty;

    public List<AttachmentModel> Attachments { get; init; } = [];

    public List<ProjectCommentModel> Comments { get; init; } = [];

    public List<ProjectSubtaskModel> Subtasks { get; init; } = [];

    public List<TaskAssigneeModel> Assignees { get; init; } = [];

    [ForeignKey(nameof(StatusId))]
    public ProjectStatusModel StatusModel { get; init; } = default!;

    [ForeignKey(nameof(CreatedBy))]
    public UserModel User { get; init; } = default!;

    [ForeignKey(nameof(ProjectId))]
    public ProjectModel ProjectModel { get; init; } = default!;
}