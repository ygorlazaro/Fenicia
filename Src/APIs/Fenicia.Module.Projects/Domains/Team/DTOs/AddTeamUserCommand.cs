using System.ComponentModel.DataAnnotations;

namespace Fenicia.Module.Projects.Domains.Team.DTOs;

public record AddTeamUserCommand(
    [Required] Guid Id,
    [Required] Guid TeamId,
    [Required] Guid UserId,
    [Required] string Role);
