using Fenicia.Common.Data.Requests.Project;
using Fenicia.Common.Enums.Project;

namespace Fenicia.Common.Data.Models.Project;

public class ProjectModel(ProjectRequest request) : BaseModel
{
    public string Title { get; set; } = request.Title;

    public string? Description { get; set; } = request.Description;

    public ProjectStatus Status { get; set; } = request.Status;

    public DateTime? StartDate { get; set; } = request.StartDate;

    public DateTime? EndDate { get; set; } = request.EndDate;

    public Guid Owner { get; set; }

    public virtual List<StatusModel> Statuses { get; set; } = [];

    public virtual List<TaskModel> Tasks { get; set; } = [];
}