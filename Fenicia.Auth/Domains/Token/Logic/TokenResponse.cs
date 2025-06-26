namespace Fenicia.Auth.Domains.Token.Logic;

/// <summary>
/// Response model containing the authentication token
/// </summary>
public class TokenResponse
{
    /// <summary>
    /// The JWT authentication token
    /// </summary>
    /// <example>eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...</example>
    public string Token { get; set; } = null!;

    /// <summary>
    /// The JWT refresh token
    /// </summary>
    /// <example>eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...</example>
    public string RefreshToken { get; set; } = null!;
}
