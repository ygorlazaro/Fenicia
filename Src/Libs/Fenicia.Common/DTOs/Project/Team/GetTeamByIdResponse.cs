using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.Project.Team;

public record GetTeamByIdResponse(
    [Required] Guid Id,
    [Required] Guid ProjectId,
    [Required] [MaxLength(128)] string Name,
    [MaxLength(2000)] string? Description,
    [MaxLength(30)] string Color,
    [Required] Guid CreatedBy,
    [Required] Guid CompanyId,
    List<TeamMemberResponse> Members);
