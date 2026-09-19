using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.Auth.Token;

public class TokenUserResponse()
{
    public TokenUserResponse(Guid id, string name, string email)
        : this()
    {
        Id = id;
        Name = name;
        Email = email;
    }

    [Required]
    public Guid Id { get; set; }

    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [MaxLength(200)]
    public string Email { get; set; } = string.Empty;
}
