using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.Project.Team;

public class UpdateTeamCommand()
{
    public UpdateTeamCommand(
        [Required] Guid id,
        [Required] [MaxLength(128)] string name,
        [MaxLength(2000)] string? description,
        [MaxLength(30)] string color)
        : this()
    {
        Id = id;
        Name = name;
        Description = description;
        Color = color;
    }

    [Required]
    public Guid Id { get; set; }

    [Required]
    [MaxLength(128)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(2000)]
    public string? Description { get; set; }

    [MaxLength(30)]
    public string Color { get; set; } = string.Empty;
}
