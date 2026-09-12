using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.Auth.RefreshToken;

public record ValidateTokenResponse(
    [Required] [MaxLength(200)] string Token,
    [Required] DateTime ExpirationDate,
    [Required] Guid UserId,
    bool IsActive);