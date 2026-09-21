using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.Project.Project;

public class ProjectResponse()
{
    public ProjectResponse(
        [Required] Guid id,
        [Required] [MaxLength(200)] string title,
        [MaxLength(200)] string? description,
        [Required] [MaxLength(200)] string status,
        DateTime? startDate,
        DateTime? endDate,
        [Required] Guid owner,
        [Required] Guid companyId,
        List<ProjectStatusResponse>? statuses = null,
        List<ProjectTaskResponse>? tasks = null)
        : this()
    {
        Id = id;
        Title = title;
        Description = description;
        Status = status;
        StartDate = startDate;
        EndDate = endDate;
        Owner = owner;
        CompanyId = companyId;
        Statuses = statuses ?? [];
        Tasks = tasks ?? [];
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
    public string Status { get; set; } = string.Empty;

    public DateTime? StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    [Required]
    public Guid Owner { get; set; }

    [Required]
    public Guid CompanyId { get; set; }

    public List<ProjectStatusResponse> Statuses { get; set; } = [];

    public List<ProjectTaskResponse> Tasks { get; set; } = [];
}