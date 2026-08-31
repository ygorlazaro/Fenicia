using System.ComponentModel.DataAnnotations;

namespace Fenicia.Auth.Domains.RefreshToken.DTOs;

public record ValidateTokenQuery([Required] Guid UserId, [Required][MaxLength(200)] string RefreshToken);
