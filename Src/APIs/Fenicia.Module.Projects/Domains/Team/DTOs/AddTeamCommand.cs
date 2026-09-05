using System.ComponentModel.DataAnnotations;

namespace Fenicia.Module.Projects.Domains.Team.DTOs;

public record AddTeamCommand(
    [Required] Guid Id,
    [Required] Guid ProjectId,
    [Required] [MaxLength(128)] string Name,
    [MaxLength(2000)] string? Description,
    [MaxLength(30)] string Color,
    [Required] Guid CreatedBy);
