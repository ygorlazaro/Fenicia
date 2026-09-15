using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.Auth.Token;

public class TokenResponse()
{
    public TokenResponse(string accessToken, string refreshToken, UserResponse user)
        : this()
    {
        AccessToken = accessToken;
        RefreshToken = refreshToken;
        User = user;
    }

    [Required]
    [MaxLength(200)]
    public string AccessToken { get; set; } = string.Empty;

    [Required]
    [MaxLength(200)]
    public string RefreshToken { get; set; } = string.Empty;

    [Required]
    public UserResponse User { get; set; } = new();
}
