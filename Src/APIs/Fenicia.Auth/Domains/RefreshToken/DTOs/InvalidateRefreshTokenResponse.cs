using System.ComponentModel.DataAnnotations;

namespace Fenicia.Auth.Domains.RefreshToken.DTOs;

public record InvalidateRefreshTokenResponse(
    [Required] [MaxLength(200)] string Token,
    [Required] DateTime ExpirationDate,
    [Required] Guid UserId)
{
    public bool IsActive { get; set; }
}