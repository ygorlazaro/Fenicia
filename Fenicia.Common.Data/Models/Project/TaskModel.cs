using Fenicia.Common.Enums.Project;

namespace Fenicia.Common.Data.Models.Project;

public class TaskModel(TaskRequest request) : BaseModel
{
    public Guid ProjectId { get; set; } = request.ProjectId;

    public Guid StatusId { get; set; } = request.StatusId;

    public string Title { get; set; } = request.Title;

    public string? Description { get; set; } = request.Description;

    public TaskPriority Priority { get; set; } = request.Priority;

    public TaskType Type { get; set; } = request.Type;

    public int Order { get; set; } = request.Order;

    public int? EstimatePoints { get; set; } = request.EstimatePoints;

    public DateTime? DueDate { get; set; } = request.DueDate;

    public Guid CreatedBy { get; set; } = request.CreatedBy;

    public virtual List<AttachmentModel> Attachments { get; set; } = [];

    public virtual List<CommentModel> Comments { get; set; } = [];

    public virtual List<SubtaskModel> Subtasks { get; set; } = [];

    public virtual List<TaskAssigneeModel> Assignees { get; set; } = [];

    public virtual StatusModel Status { get; set; } = null!;

    public virtual UserModel User { get; set; } = null!;

    public virtual ProjectModel Project { get; set; } = null!;
}