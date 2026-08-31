using System.ComponentModel.DataAnnotations;

namespace Fenicia.Auth.Domains.RefreshToken.DTOs;

public sealed record GenerateRefreshTokenCommand([Required] Guid UserId);
