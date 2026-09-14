using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.Project.Team;

public record UpdateTeamUserRoleCommand(
    [Required] Guid TeamId,
    [Required] Guid UserId,
    [Required] string Role);
