using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.Auth.RefreshToken;

public class GenerateRefreshTokenResponse()
{
    public GenerateRefreshTokenResponse(string token, DateTime expirationDate)
        : this()
    {
        Token = token;
        ExpirationDate = expirationDate;
    }

    [Required]
    [MaxLength(200)]
    public string Token { get; set; } = string.Empty;

    [Required]
    public DateTime ExpirationDate { get; set; }
}
