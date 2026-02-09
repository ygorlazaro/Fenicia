using Fenicia.Common.Data.Requests.Project;

namespace Fenicia.Common.Data.Models.Project;

public class SubtaskModel(SubtaskRequest request) : BaseModel
{
    public Guid TaskId { get; set; } = request.TaskId;

    public string Title { get; set; } = request.Title;

    public bool IsCompleted { get; set; } = request.IsCompleted;

    public int Order { get; set; } = request.Order;

    public DateTime? CompletedAt { get; set; }

    public virtual TaskModel Task { get; set; } = null!;
}