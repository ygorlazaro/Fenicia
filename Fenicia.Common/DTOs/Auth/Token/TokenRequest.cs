using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.Auth.Token;

/// <summary>
/// Request DTO for token generation containing user credentials.
/// </summary>
public class TokenRequest()
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TokenRequest"/> class with the specified email and password.
    /// </summary>
    /// <param name="email">The user's email address.</param>
    /// <param name="password">The user's password.</param>
    public TokenRequest(string email, string password)
        : this()
    {
        Email = email;
        Password = password;
    }

    /// <summary>
    /// Gets or sets the user's email address.
    /// </summary>
    [Required]
    [MaxLength(200)]
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the user's password.
    /// </summary>
    [Required]
    [MaxLength(200)]
    public string Password { get; set; } = string.Empty;
}
