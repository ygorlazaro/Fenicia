using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.Project.Team;

public record AddTeamUserResponse(
    [Required] Guid Id,
    [Required] Guid TeamId,
    [Required] Guid UserId,
    [Required] string Role,
    [Required] DateTime JoinedAt,
    [Required] Guid CompanyId);
