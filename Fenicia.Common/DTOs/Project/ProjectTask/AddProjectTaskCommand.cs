using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.Project.ProjectTask;

public class AddProjectTaskCommand()
{
    public AddProjectTaskCommand(
        [Required] Guid id,
        [Required] Guid projectId,
        [Required] Guid statusId,
        [Required] [MaxLength(200)] string title,
        [MaxLength(200)] string? description,
        [Required] [MaxLength(200)] string priority,
        [Required] [MaxLength(200)] string type,
        int order,
        int? estimatePoints,
        DateTime? dueDate,
        [Required] Guid createdBy,
        Guid? sprintId)
        : this()
    {
        Id = id;
        ProjectId = projectId;
        StatusId = statusId;
        Title = title;
        Description = description;
        Priority = priority;
        Type = type;
        Order = order;
        EstimatePoints = estimatePoints;
        DueDate = dueDate;
        CreatedBy = createdBy;
        SprintId = sprintId;
    }

    [Required]
    public Guid Id { get; set; }

    [Required]
    public Guid ProjectId { get; set; }

    [Required]
    public Guid StatusId { get; set; }

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

    public int Order { get; set; }

    public int? EstimatePoints { get; set; }

    public DateTime? DueDate { get; set; }

    [Required]
    public Guid CreatedBy { get; set; }

    public Guid? SprintId { get; set; }
}
