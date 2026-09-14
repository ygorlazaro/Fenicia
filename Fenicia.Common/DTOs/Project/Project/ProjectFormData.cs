namespace Fenicia.Common.DTOs.Project.Project;

public record ProjectFormData
{
    public Guid Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string? Description { get; set; }

    public string Status { get; set; } = "Active";

    public DateTime? StartDate { get; set; }

    public DateTime? EndDate { get; set; }
}
