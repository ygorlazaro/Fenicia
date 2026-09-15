using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.Auth.RefreshToken;

public class InvalidateRefreshTokenResponse()
{
    public InvalidateRefreshTokenResponse(string token, DateTime expirationDate, Guid userId)
        : this()
    {
        Token = token;
        ExpirationDate = expirationDate;
        UserId = userId;
    }

    [Required]
    [MaxLength(200)]
    public string Token { get; set; } = string.Empty;

    [Required]
    public DateTime ExpirationDate { get; set; }

    [Required]
    public Guid UserId { get; set; }

    public bool IsActive { get; set; }
}
