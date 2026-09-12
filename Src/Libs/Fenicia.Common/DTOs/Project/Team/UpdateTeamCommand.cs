using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.Project.Team;

public record UpdateTeamCommand(
    [Required] Guid Id,
    [Required] [MaxLength(128)] string Name,
    [MaxLength(2000)] string? Description,
    [MaxLength(30)] string Color);
