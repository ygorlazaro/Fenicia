using Fenicia.Common.Data.Models.Project;
using Fenicia.Common.Enums.Project;

namespace Fenicia.Common.Data.Responses.Project;

public class TaskResponse(TaskModel model)
{
    public Guid Id { get; } = model.Id;

    public Guid ProjectId { get; } = model.ProjectId;

    public Guid StatusId { get; } = model.StatusId;

    public string Title { get; } = model.Title;

    public string? Description { get; } = model.Description;

    public TaskPriority Priority { get; } = model.Priority;

    public TaskType Type { get; } = model.Type;

    public int Order { get; } = model.Order;

    public int? EstimatePoints { get; } = model.EstimatePoints;

    public DateTime? DueDate { get; } = model.DueDate;

    public Guid CreatedBy { get; } = model.CreatedBy;
}