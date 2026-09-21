using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.Project.Project;

public class ProjectRequest()
{
    public ProjectRequest(
        Guid? id,
        [Required] [MaxLength(200)] string title,
        [MaxLength(200)] string? description,
        [Required] [MaxLength(200)] string status,
        DateTime? startDate,
        DateTime? endDate,
        [Required] Guid owner)
        : this()
    {
        Id = id;
        Title = title;
        Description = description;
        Status = status;
        StartDate = startDate;
        EndDate = endDate;
        Owner = owner;
    }

    public Guid? Id { get; set; }

    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [MaxLength(200)]
    public string? Description { get; set; }

    [Required]
    [MaxLength(200)]
    public string Status { get; set; } = string.Empty;

    public DateTime? StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    [Required]
    public Guid Owner { get; set; }
}