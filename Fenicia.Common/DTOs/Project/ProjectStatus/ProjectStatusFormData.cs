namespace Fenicia.Common.DTOs.Project.ProjectStatus;

public class ProjectStatusFormData
{
    public Guid Id { get; set; }

    public Guid ProjectId { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Color { get; set; } = "#6366f1";

    public int Order { get; set; }

    public bool IsFinal { get; set; }
}
