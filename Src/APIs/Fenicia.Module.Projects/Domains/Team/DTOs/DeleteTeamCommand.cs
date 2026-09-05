using System.ComponentModel.DataAnnotations;

namespace Fenicia.Module.Projects.Domains.Team.DTOs;

public record DeleteTeamCommand([Required] Guid Id);
