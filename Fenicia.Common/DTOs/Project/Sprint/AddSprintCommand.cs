using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.Project.Sprint;

public class AddSprintCommand()
{
    public AddSprintCommand(
        [Required] Guid id,
        [Required] Guid projectId,
        [Required] [MaxLength(256)] string name,
        DateTime? startDate,
        DateTime? endDate,
        [MaxLength(4096)] string? description,
        [Required] Guid createdBy)
        : this()
    {
        Id = id;
        ProjectId = projectId;
        Name = name;
        StartDate = startDate;
        EndDate = endDate;
        Description = description;
        CreatedBy = createdBy;
    }

    [Required]
    public Guid Id { get; set; }

    [Required]
    public Guid ProjectId { get; set; }

    [Required]
    [MaxLength(256)]
    public string Name { get; set; } = string.Empty;

    public DateTime? StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    [MaxLength(4096)]
    public string? Description { get; set; }

    [Required]
    public Guid CreatedBy { get; set; }
}
