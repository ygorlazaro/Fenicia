using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.Project.Project;

public class ProjectTaskResponse()
{
    public ProjectTaskResponse(
        [Required] Guid id,
        [Required] [MaxLength(200)] string title,
        [MaxLength(200)] string? description,
        [Required] [MaxLength(200)] string priority,
        [Required] [MaxLength(200)] string type,
        int? estimatePoints,
        DateTime? dueDate)
        : this()
    {
        Id = id;
        Title = title;
        Description = description;
        Priority = priority;
        Type = type;
        EstimatePoints = estimatePoints;
        DueDate = dueDate;
    }

    [Required]
    public Guid Id { get; set; }

    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [MaxLength(200)]
    public string? Description { get; set; }

    [Required]
    [MaxLength(200)]
    public string Priority { get; set; } = string.Empty;

    [Required]
    [MaxLength(200)]
    public string Type { get; set; } = string.Empty;

    public int? EstimatePoints { get; set; }

    public DateTime? DueDate { get; set; }
}
