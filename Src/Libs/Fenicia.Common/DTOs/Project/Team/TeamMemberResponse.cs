using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.Project.Team;

public record TeamMemberResponse(
    [Required] Guid UserId,
    [Required] [MaxLength(64)] string UserName,
    [Required] [MaxLength(48)] string Email,
    [Required] string Role,
    [Required] DateTime JoinedAt);
