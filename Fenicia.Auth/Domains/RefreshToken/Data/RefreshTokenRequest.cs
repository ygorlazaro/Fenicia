namespace Fenicia.Auth.Domains.RefreshToken.Data;

using System.ComponentModel.DataAnnotations;

/// <summary>
///     Represents a request to refresh an authentication token.
/// </summary>
public class RefreshTokenRequest
{
    /// <summary>
    ///     Gets or sets the current access token that needs to be refreshed.
    /// </summary>
    [Required]
    [MaxLength(length: 2048)]
    public string AccessToken { get; set; } = string.Empty;

    /// <summary>
    ///     Gets or sets the refresh token used to obtain a new access token.
    /// </summary>
    [Required]
    [MaxLength(length: 2048)]
    public string RefreshToken { get; set; } = string.Empty;

    /// <summary>
    ///     Gets or sets the unique identifier of the user requesting the token refresh.
    /// </summary>
    [Required]
    public Guid UserId { get; set; }

    /// <summary>
    ///     Gets or sets the unique identifier of the company associated with the user.
    /// </summary>
    [Required]
    public Guid CompanyId { get; set; }
}
