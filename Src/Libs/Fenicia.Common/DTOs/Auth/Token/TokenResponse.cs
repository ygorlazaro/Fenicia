using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.Auth.Token;

public record TokenResponse(
    [Required] [MaxLength(200)] string AccessToken,
    [Required] [MaxLength(200)] string RefreshToken,
    UserResponse User);