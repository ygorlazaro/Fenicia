using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.Project.Team;

public class AddTeamUserResponse()
{
    public AddTeamUserResponse(
        [Required] Guid id,
        [Required] Guid teamId,
        [Required] Guid userId,
        [Required] string role,
        [Required] DateTime joinedAt,
        [Required] Guid companyId)
        : this()
    {
        Id = id;
        TeamId = teamId;
        UserId = userId;
        Role = role;
        JoinedAt = joinedAt;
        CompanyId = companyId;
    }

    [Required]
    public Guid Id { get; set; }

    [Required]
    public Guid TeamId { get; set; }

    [Required]
    public Guid UserId { get; set; }

    [Required]
    public string Role { get; set; } = string.Empty;

    [Required]
    public DateTime JoinedAt { get; set; }

    [Required]
    public Guid CompanyId { get; set; }
}
