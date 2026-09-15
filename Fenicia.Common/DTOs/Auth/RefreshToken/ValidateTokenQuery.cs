using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.Auth.RefreshToken;

public class ValidateTokenQuery()
{
    public ValidateTokenQuery(Guid userId, string refreshToken)
        : this()
    {
        UserId = userId;
        RefreshToken = refreshToken;
    }

    [Required]
    public Guid UserId { get; set; }

    [Required]
    [MaxLength(200)]
    public string RefreshToken { get; set; } = string.Empty;
}
