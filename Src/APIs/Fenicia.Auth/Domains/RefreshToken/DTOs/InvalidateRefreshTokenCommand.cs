using System.ComponentModel.DataAnnotations;

namespace Fenicia.Auth.Domains.RefreshToken.DTOs;

public sealed record InvalidateRefreshTokenCommand([Required] [MaxLength(200)] string RefreshToken);