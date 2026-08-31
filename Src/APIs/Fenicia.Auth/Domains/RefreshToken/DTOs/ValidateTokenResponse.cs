using System.ComponentModel.DataAnnotations;

namespace Fenicia.Auth.Domains.RefreshToken.DTOs;

public record ValidateTokenResponse([Required][MaxLength(200)] string Token, [Required] DateTime ExpirationDate, [Required] Guid UserId, bool IsActive);
