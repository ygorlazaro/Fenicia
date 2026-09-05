using System.ComponentModel.DataAnnotations;

namespace Fenicia.Module.Projects.Domains.Team.DTOs;

public record RemoveTeamUserCommand([Required] Guid TeamId, [Required] Guid UserId);
