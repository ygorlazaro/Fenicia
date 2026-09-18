using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.Project.Team;

public class TeamMemberResponse()
{
    public TeamMemberResponse(
        [Required] Guid userId,
        [Required] [MaxLength(64)] string userName,
        [Required] [MaxLength(48)] string email,
        [Required] string role,
        [Required] DateTime joinedAt)
        : this()
    {
        UserId = userId;
        UserName = userName;
        Email = email;
        Role = role;
        JoinedAt = joinedAt;
    }

    [Required]
    public Guid UserId { get; set; }

    [Required]
    [MaxLength(64)]
    public string UserName { get; set; } = string.Empty;

    [Required]
    [MaxLength(48)]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string Role { get; set; } = string.Empty;

    [Required]
    public DateTime JoinedAt { get; set; }
}
