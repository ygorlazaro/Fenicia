namespace Fenicia.Common.Database.Requests;

using System.ComponentModel.DataAnnotations;

public class RefreshTokenRequest
{
    [Required]
    [MaxLength(length: 2048)]
    public string AccessToken { get; set; } = string.Empty;

    [Required]
    [MaxLength(length: 2048)]
    public string RefreshToken { get; set; } = string.Empty;

    [Required]
    public Guid UserId
    {
        get; set;
    }

    [Required]
    public Guid CompanyId
    {
        get; set;
    }
}
