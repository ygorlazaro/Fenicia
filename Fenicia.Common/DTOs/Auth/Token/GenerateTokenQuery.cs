using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.Auth.Token;

public record GenerateTokenQuery
{
    public GenerateTokenQuery()
    {
    }

    public GenerateTokenQuery(string email, string password)
    {
        Email = email;
        Password = password;
    }

    [Required]
    [MaxLength(200)]
    public string Email { get; set; } = string.Empty;

    [Required]
    [MaxLength(200)]
    public string Password { get; set; } = string.Empty;
}
