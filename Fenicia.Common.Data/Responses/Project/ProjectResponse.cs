using Fenicia.Common.Data.Models.Project;
using Fenicia.Common.Enums.Project;

namespace Fenicia.Common.Data.Responses.Project;

public class ProjectResponse(ProjectModel project)
{
    public Guid Owner { get; set; } = project.Owner;

    public DateTime? EndDate { get; set; } = project.EndDate;

    public DateTime? StartDate { get; set; } = project.StartDate;

    public ProjectStatus Status { get; set; } = project.Status;

    public string? Description { get; set; } = project.Description;

    public string Title { get; set; } = project.Title;

    public Guid Id { get; set; } = project.Id;
}