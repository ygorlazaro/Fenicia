using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.Project.Team;

public record GetTeamByIdQuery([Required] Guid Id);
