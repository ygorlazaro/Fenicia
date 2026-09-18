namespace Fenicia.Common.DTOs.Project.Team;

public class TeamFormData
{
    public Guid Id { get; set; }

    public Guid ProjectId { get; set; }

    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public string Color { get; set; } = "#6366f1";
}
