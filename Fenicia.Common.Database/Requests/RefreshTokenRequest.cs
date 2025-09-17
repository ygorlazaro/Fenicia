namespace Fenicia.Common.Database.Requests;

using System.ComponentModel.DataAnnotations;

public class RefreshTokenRequest
{
    [Required(ErrorMessage = "Access token is required")]
    [MaxLength(length: 2048)]
    public string AccessToken { get; set; } = string.Empty;

    [Required(ErrorMessage = "Refresh token is required")]
    [MaxLength(length: 2048)]
    public string RefreshToken { get; set; } = string.Empty;

    [Required(ErrorMessage = "User ID is required")]
    public Guid UserId
    {
        get; set;
    }

    [Required(ErrorMessage = "Company ID is required")]
    public Guid CompanyId
    {
        get; set;
    }
}
