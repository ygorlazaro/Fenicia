using Fenicia.Common.Enums.Project;

namespace Fenicia.Common.Data.Models.Project;

public class ProjectModel : BaseModel
{
    public string Title { get; set; }

    public string? Description { get; set; }

    public ProjectStatus Status { get; set; }

    public DateTime? StartDate { get; set; }

    public DateTime? EndDate { get; set; }
}