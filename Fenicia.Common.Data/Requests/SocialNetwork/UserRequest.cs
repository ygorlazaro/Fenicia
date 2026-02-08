using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.Data.Requests.SocialNetwork;

public class UserRequest
{
    [Required]
    [MaxLength(50)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [MaxLength(48)]
    public string Email { get; set; } = string.Empty;

    [Required]
    [MaxLength(48)]
    public string Username { get; set; } = string.Empty;

    public string? ImageUrl { get; set; }

    public Guid UserId { get; set; }
}