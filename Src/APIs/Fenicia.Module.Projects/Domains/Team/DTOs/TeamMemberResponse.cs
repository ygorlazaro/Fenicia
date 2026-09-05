using System.ComponentModel.DataAnnotations;

namespace Fenicia.Module.Projects.Domains.Team.DTOs;

public record TeamMemberResponse(
    [Required] Guid UserId,
    [Required] [MaxLength(64)] string UserName,
    [Required] [MaxLength(48)] string Email,
    [Required] string Role,
    [Required] DateTime JoinedAt);
