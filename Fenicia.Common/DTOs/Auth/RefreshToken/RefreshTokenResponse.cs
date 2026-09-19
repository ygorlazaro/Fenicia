using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.Auth.RefreshToken;

public class RefreshTokenResponse()
{
    public RefreshTokenResponse(string token, DateTime expirationDate, Guid userId, bool isActive)
        : this()
    {
        Token = token;
        ExpirationDate = expirationDate;
        UserId = userId;
        IsActive = isActive;
    }

    [Required]
    [MaxLength(200)]
    public string Token { get; set; } = string.Empty;

    [Required]
    public DateTime ExpirationDate { get; set; }

    [Required]
    public Guid UserId { get; set; }

    [Required]
    public bool IsActive { get; set; }
}
