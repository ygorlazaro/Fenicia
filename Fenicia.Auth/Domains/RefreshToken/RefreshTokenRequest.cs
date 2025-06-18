using System.ComponentModel.DataAnnotations;

namespace Fenicia.Auth.Domains.RefreshToken;

public class RefreshTokenRequest
{
    [Required]
    public string AccessToken { get; set; } = null!;

    [Required]
    public string RefreshToken { get; set; } = null!;
}
