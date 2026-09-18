namespace Fenicia.Common.DTOs.Project.Sprint;

public class SprintFormData
{
    public Guid Id { get; set; }

    public Guid ProjectId { get; set; }

    public string Name { get; set; } = string.Empty;

    public DateTime? StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public string? Description { get; set; }

    public Guid CreatedBy { get; set; }
}
