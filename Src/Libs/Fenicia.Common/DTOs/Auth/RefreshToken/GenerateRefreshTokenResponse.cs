using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.Auth.RefreshToken;

public record GenerateRefreshTokenResponse(
    [Required] [MaxLength(200)] string Token,
    [Required] DateTime ExpirationDate);