namespace Fenicia.Auth.Domains.Token.Logic;

/// <summary>
/// Request model for authentication token generation
/// </summary>
public record TokenRequest
{
    /// <summary>
    /// User's email address
    /// </summary>
    /// <example>user@example.com</example>
    public string Email { get; set; } = null!;

    /// <summary>
    /// User's password
    /// </summary>
    public string Password { get; set; } = null!;

    /// <summary>
    /// Company's CNPJ number
    /// </summary>
    /// <example>12345678000199</example>
    public string Cnpj { get; set; } = null!;
}
