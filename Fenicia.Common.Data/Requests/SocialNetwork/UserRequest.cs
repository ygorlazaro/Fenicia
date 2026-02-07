using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.Data.Requests.SocialNetwork;

public class UserRequest
{
    [Required]
    [MaxLength(50)]
    public string Name { get; set; }

    [Required]
    [EmailAddress]
    [MaxLength(48)]
    public string Email { get; set; }

    [Required]
    [MaxLength(48)]
    public string Username { get; set; }

    public string? ImageUrl { get; set; }
}