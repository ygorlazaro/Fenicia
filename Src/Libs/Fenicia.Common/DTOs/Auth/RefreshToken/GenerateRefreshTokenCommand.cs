using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.Auth.RefreshToken;

public sealed record GenerateRefreshTokenCommand([Required] Guid UserId);