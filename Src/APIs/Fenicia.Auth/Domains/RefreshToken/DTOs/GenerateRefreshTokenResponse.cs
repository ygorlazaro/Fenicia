using System.ComponentModel.DataAnnotations;

namespace Fenicia.Auth.Domains.RefreshToken.DTOs;

public record GenerateRefreshTokenResponse([Required][MaxLength(200)] string Token, [Required] DateTime ExpirationDate);
