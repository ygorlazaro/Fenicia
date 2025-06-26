namespace Fenicia.Auth.Domains.Token.Data;

using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

/// <summary>
///     Response model containing the authentication token and refresh token for user authentication.
/// </summary>
/// <remarks>
///     This class represents the response returned after successful authentication,
///     containing both the main JWT token and a refresh token for maintaining session.
/// </remarks>
[DataContract]
public class TokenResponse
{
    /// <summary>
    ///     The JWT authentication token used for authorizing requests.
    /// </summary>
    /// <example>eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...</example>
    /// <remarks>
    ///     This token should be included in the Authorization header of subsequent requests.
    /// </remarks>
    [Required]
    [StringLength(maximumLength: 2048)]
    [DataMember(Name = "token")]
    public string Token { get; set; } = null!;

    /// <summary>
    ///     The JWT refresh token used for obtaining new authentication tokens.
    /// </summary>
    /// <example>eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...</example>
    /// <remarks>
    ///     This token is used to request a new authentication token when the current one expires.
    /// </remarks>
    [Required]
    [StringLength(maximumLength: 2048)]
    [DataMember(Name = "refreshToken")]
    public string RefreshToken { get; set; } = null!;
}
