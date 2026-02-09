using Fenicia.Common.Enums.Project;

namespace Fenicia.Common.Data.Requests.Project;

public class ProjectRequest
{
    public string Title { get; set; } = string.Empty;

    public string? Description { get; set; }

    public ProjectStatus Status { get; set; }

    public DateTime? StartDate { get; set; }

    public DateTime? EndDate { get; set; }
}