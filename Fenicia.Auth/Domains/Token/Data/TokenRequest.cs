namespace Fenicia.Auth.Domains.Token.Data;

using System.ComponentModel.DataAnnotations;

/// <summary>
///     Request model for authentication token generation used in the authentication process
///     Contains the necessary user credentials and company information for token issuance
/// </summary>
public record TokenRequest
{
    /// <summary>
    ///     The email address of the user attempting to authenticate
    ///     Must be a valid email format
    /// </summary>
    /// <example>user@example.com</example>
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email address format")]
    [StringLength(maximumLength: 256, ErrorMessage = "Email must not exceed 256 characters")]
    [Display(Name = "Email Address")]
    public string Email { get; set; } = null!;

    /// <summary>
    ///     The password associated with the user account
    ///     Used for authentication verification
    /// </summary>
    [Required(ErrorMessage = "Password is required")]
    [StringLength(maximumLength: 100, MinimumLength = 6, ErrorMessage = "Password must be between 6 and 100 characters")]
    [Display(Name = "Password")]
    public string Password { get; set; } = null!;

    /// <summary>
    ///     The CNPJ (Brazilian Company Registration Number) of the company
    ///     Must be a valid 14-digit number without special characters
    /// </summary>
    /// <example>12345678000199</example>
    [Required(ErrorMessage = "CNPJ is required")]
    [StringLength(maximumLength: 14, MinimumLength = 14, ErrorMessage = "CNPJ must be exactly 14 digits")]
    [Display(Name = "CNPJ")]
    public string Cnpj { get; set; } = null!;
}
