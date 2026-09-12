using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.Auth.RefreshToken;

public record ValidateTokenQuery([Required] Guid UserId, [Required] [MaxLength(200)] string RefreshToken);