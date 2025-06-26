namespace Fenicia.Auth.Domains.User.Data;

using System.ComponentModel.DataAnnotations;

using Company.Data;

/// <summary>
///     Request model for user registration and management
/// </summary>
/// <remarks>
///     This class represents the data contract for user-related operations
/// </remarks>
public class UserRequest
{
    /// <summary>
    ///     User's email address used for authentication and communication
    /// </summary>
    /// <example>user@example.com</example>
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email address format")]
    [MaxLength(length: 48, ErrorMessage = "Email cannot exceed 48 characters")]
    public string Email { get; set; } = null!;

    /// <summary>
    ///     User's password for account security
    /// </summary>
    /// <example>StrongP@ssw0rd123</example>
    [Required(ErrorMessage = "Password is required")]
    [MinLength(length: 8, ErrorMessage = "Password must be at least 8 characters")]
    [MaxLength(length: 200, ErrorMessage = "Password cannot exceed 200 characters")]
    public string Password { get; set; } = null!;

    /// <summary>
    ///     User's full name for identification
    /// </summary>
    /// <example>John Doe</example>
    [Required(ErrorMessage = "Name is required")]
    [MaxLength(length: 48, ErrorMessage = "Name cannot exceed 48 characters")]
    public string Name { get; set; } = null!;

    /// <summary>
    ///     Company information associated with the user
    /// </summary>
    [Required(ErrorMessage = "Company information is required")]
    public CompanyRequest Company { get; set; } = null!;
}
