using System.ComponentModel.DataAnnotations;

namespace Fenicia.Module.Projects.Domains.Team.DTOs;

public record GetTeamByIdQuery([Required] Guid Id);
