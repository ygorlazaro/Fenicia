using Fenicia.Common.Data.Requests.Project;

namespace Fenicia.Common.Data.Models.Project;

public class StatusModel(StatusRequest request) : BaseModel
{
    public Guid ProjectId { get; set; } = request.ProjectId;

    public string Name { get; set; } = request.Name;

    public string Color { get; set; } = request.Color;

    public int Order { get; set; } = request.Order;

    public bool IsFinal { get; set; } = request.IsFinal;

    public virtual ProjectModel Project { get; set; } = null!;

    public virtual List<TaskModel> Tasks { get; set; } = [];
}
