using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.Project.ProjectStatus;

public class AddProjectStatusResponse()
{
    public AddProjectStatusResponse(
        [Required] Guid id,
        [Required] Guid projectId,
        [Required] [MaxLength(200)] string name,
        [Required] [MaxLength(200)] string color,
        int order,
        bool isFinal,
        [Required] Guid companyId)
        : this()
    {
        Id = id;
        ProjectId = projectId;
        Name = name;
        Color = color;
        Order = order;
        IsFinal = isFinal;
        CompanyId = companyId;
    }

    [Required]
    public Guid Id { get; set; }

    [Required]
    public Guid ProjectId { get; set; }

    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [MaxLength(200)]
    public string Color { get; set; } = string.Empty;

    public int Order { get; set; }

    public bool IsFinal { get; set; }

    [Required]
    public Guid CompanyId { get; set; }
}
