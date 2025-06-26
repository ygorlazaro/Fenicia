using System.ComponentModel.DataAnnotations;

namespace Fenicia.Auth.Domains.ForgotPassword.Data;

/// <summary>
/// Represents a request model for resetting a forgotten password using a verification code.
/// This class encapsulates the data required to complete the password reset process.
/// </summary>
/// <remarks>
/// The model includes validation attributes to ensure data integrity and security requirements are met.
/// All properties are required and have specific validation rules.
/// </remarks>
public class ForgotPasswordRequestReset
{
    /// <summary>
    /// Gets or sets the email address associated with the user account.
    /// </summary>
    /// <remarks>
    /// The email address must be:
    /// - In a valid email format
    /// - Match an existing user account in the system
    /// - The same email where the verification code was sent
    /// </remarks>
    /// <value>A string containing the user's email address.</value>
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email address format")]
    public string Email { get; set; } = null!;

    /// <summary>
    /// Gets or sets the verification code sent to the user's email for password reset validation.
    /// </summary>
    /// <remarks>
    /// The verification code must be:
    /// - At least 6 characters long
    /// - Match the code stored in the system
    /// - Not expired
    /// - Not previously used
    /// </remarks>
    /// <value>A string containing the verification code.</value>
    [Required(ErrorMessage = "Verification code is required")]
    [MinLength(6, ErrorMessage = "Verification code must be at least 6 characters")]
    public string Code { get; set; } = null!;

    /// <summary>
    /// Gets or sets the new password for the user account.
    /// </summary>
    /// <remarks>
    /// The password must meet the following security requirements:
    /// - Minimum length of 8 characters
    /// - At least one uppercase letter
    /// - At least one lowercase letter
    /// - At least one number
    /// - At least one special character (@$!%*?&amp;)
    /// </remarks>
    /// <value>A string containing the new password that meets security requirements.</value>
    [Required(ErrorMessage = "Password is required")]
    [MinLength(8, ErrorMessage = "Password must be at least 8 characters")]
    [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$",
        ErrorMessage = "Password must contain at least one uppercase letter, one lowercase letter, one number and one special character")]
    public string Password { get; set; } = null!;
}
