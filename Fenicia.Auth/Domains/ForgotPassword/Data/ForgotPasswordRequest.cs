namespace Fenicia.Auth.Domains.ForgotPassword.Data;

using System.ComponentModel.DataAnnotations;

/// <summary>
///     Represents a request model for initiating a password recovery process.
/// </summary>
/// <remarks>
///     This class encapsulates the necessary information required to begin the password recovery workflow.
///     It includes validation to ensure the provided email is properly formatted and present.
/// </remarks>
public class ForgotPasswordRequest
{
    /// <summary>
    ///     Gets or sets the email address associated with the user account for password recovery.
    /// </summary>
    /// <remarks>
    ///     The email address must:
    ///     - Be a valid email format
    ///     - Be registered in the system
    ///     - Not be empty or null
    /// </remarks>
    /// <value>The email address of the user requesting password recovery.</value>
    [Required(ErrorMessage = "Email address is required")]
    [EmailAddress(ErrorMessage = "Invalid email address format")]
    [StringLength(maximumLength: 256, ErrorMessage = "Email address cannot exceed 256 characters")]
    [RegularExpression(pattern: @"^[^@\s]+@[^@\s]+\.[^@\s]+$", ErrorMessage = "Email address format is invalid")]
    public string Email { get; set; } = null!;
}
