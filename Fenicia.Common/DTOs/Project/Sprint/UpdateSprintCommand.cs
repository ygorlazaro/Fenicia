using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.Project.Sprint;

public class UpdateSprintCommand()
{
    public UpdateSprintCommand(
        [Required] Guid id,
        [Required] [MaxLength(256)] string name,
        DateTime? startDate,
        DateTime? endDate,
        [MaxLength(4096)] string? description)
        : this()
    {
        Id = id;
        Name = name;
        StartDate = startDate;
        EndDate = endDate;
        Description = description;
    }

    [Required]
    public Guid Id { get; set; }

    [Required]
    [MaxLength(256)]
    public string Name { get; set; } = string.Empty;

    public DateTime? StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    [MaxLength(4096)]
    public string? Description { get; set; }
}
