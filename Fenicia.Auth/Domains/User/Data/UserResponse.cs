using System.ComponentModel.DataAnnotations;

namespace Fenicia.Auth.Domains.User.Data;

/// <summary>
/// Represents a response model containing user information
/// </summary>
/// <remarks>
/// This class is used to transfer user data in API responses
/// </remarks>
public class UserResponse
{
    /// <summary>
    /// Gets or sets the full name of the user
    /// </summary>
    /// <example>John Doe</example>
    /// <remarks>
    /// The name should be in a human-readable format
    /// </remarks>
    [Required]
    [StringLength(48, MinimumLength = 2)]
    [Display(Name = "Full Name")]
    public string Name { get; set; } = null!;

    /// <summary>
    /// Gets or sets the email address of the user
    /// </summary>
    /// <example>user@example.com</example>
    /// <remarks>
    /// The email address must be in a valid format
    /// </remarks>
    [Required]
    [EmailAddress]
    [StringLength(48)]
    [Display(Name = "Email Address")]
    public string Email { get; set; } = null!;

    /// <summary>
    /// Gets or sets the unique identifier of the user
    /// </summary>
    /// <example>550e8400-e29b-41d4-a716-446655440000</example>
    /// <remarks>
    /// This is the primary key used to identify the user
    /// </remarks>
    [Required]
    [Display(Name = "User ID")]
    public Guid Id { get; set; }
}
