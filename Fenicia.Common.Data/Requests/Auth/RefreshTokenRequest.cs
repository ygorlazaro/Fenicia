using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.Data.Requests.Auth;

public class RefreshTokenRequest
{
    [Required(ErrorMessage = "Refresh token is required")]
    [MaxLength(2048)]
    public string RefreshToken { get; set; } = string.Empty;

    [Required(ErrorMessage = "User ID is required")]
    public Guid UserId
    {
        get; set;
    }
}
