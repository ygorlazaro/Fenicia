using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.Project.Team;

public record RemoveTeamUserCommand([Required] Guid TeamId, [Required] Guid UserId);
