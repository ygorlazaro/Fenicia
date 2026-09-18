using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.Project.Project;

public class ProjectStatusResponse()
{
    public ProjectStatusResponse(
        [Required] Guid id,
        [Required] [MaxLength(200)] string name,
        [Required] [MaxLength(200)] string color,
        int order,
        bool isFinal)
        : this()
    {
        Id = id;
        Name = name;
        Color = color;
        Order = order;
        IsFinal = isFinal;
    }

    [Required]
    public Guid Id { get; set; }

    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [MaxLength(200)]
    public string Color { get; set; } = string.Empty;

    public int Order { get; set; }

    public bool IsFinal { get; set; }
}
