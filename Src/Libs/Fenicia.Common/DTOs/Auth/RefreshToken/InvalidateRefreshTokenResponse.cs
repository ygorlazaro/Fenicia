using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.Auth.RefreshToken;

public record InvalidateRefreshTokenResponse(
    [Required] [MaxLength(200)] string Token,
    [Required] DateTime ExpirationDate,
    [Required] Guid UserId)
{
    public bool IsActive { get; set; }
}