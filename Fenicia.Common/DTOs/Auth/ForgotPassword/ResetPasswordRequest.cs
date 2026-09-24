using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.Auth.ForgotPassword;

/// <summary>
/// Request DTO for resetting a user's password.
/// </summary>
public class ResetPasswordRequest()
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ResetPasswordRequest"/> class with the specified details.
    /// </summary>
    /// <param name="email">The user's email address.</param>
    /// <param name="password">The new password.</param>
    /// <param name="code">The reset code sent to the user.</param>
    public ResetPasswordRequest(string email, string password, string code)
        : this()
    {
        Email = email;
        Password = password;
        Code = code;
    }

    /// <summary>
    /// Gets or sets the user's email address.
    /// </summary>
    [Required]
    [EmailAddress]
    [MaxLength(48)]
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the new password.
    /// </summary>
    [Required]
    [MaxLength(200)]
    public string Password { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the reset code sent to the user.
    /// </summary>
    [Required]
    [StringLength(6)]
    public string Code { get; set; } = string.Empty;
}
