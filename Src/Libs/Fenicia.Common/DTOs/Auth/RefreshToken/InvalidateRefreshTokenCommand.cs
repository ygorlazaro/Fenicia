using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.Auth.RefreshToken;

public sealed record InvalidateRefreshTokenCommand([Required] [MaxLength(200)] string RefreshToken);