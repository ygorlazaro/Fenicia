using System.ComponentModel.DataAnnotations;

namespace Fenicia.Auth.Domains.Token.DTOs;

public record TokenResponse(
    [Required] [MaxLength(200)] string AccessToken,
    [Required] [MaxLength(200)] string RefreshToken,
    UserResponse User);