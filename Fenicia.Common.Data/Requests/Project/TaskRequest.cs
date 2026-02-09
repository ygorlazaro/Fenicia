using Fenicia.Common.Enums.Project;

public class TaskRequest
{
    public Guid ProjectId { get; set; }

    public Guid StatusId { get; set; }

    public string Title { get; set; } = string.Empty;

    public string? Description { get; set; }

    public TaskPriority Priority { get; set; }

    public TaskType Type { get; set; }

    public int Order { get; set; }

    public int? EstimatePoints { get; set; }

    public DateTime? DueDate { get; set; }

    public Guid CreatedBy { get; set; }
}