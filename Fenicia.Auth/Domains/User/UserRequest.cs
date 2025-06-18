using System.ComponentModel.DataAnnotations;

using Fenicia.Auth.Domains.Company;

namespace Fenicia.Auth.Domains.User;

/// <summary>
/// Request model for user registration
/// </summary>
public class UserRequest
{
    /// <summary>
    /// User's email address
    /// </summary>
    /// <example>user@example.com</example>
    [EmailAddress]
    [Required]
    public string Email { get; set; } = null!;

    /// <summary>
    /// User's password
    /// </summary>
    [Required]
    [MaxLength(48)]
    public string Password { get; set; } = null!;

    /// <summary>
    /// User's full name
    /// </summary>
    /// <example>John Doe</example>
    [Required]
    [MaxLength(32)]
    public string Name { get; set; } = null!;

    /// <summary>
    /// Company information for the user
    /// </summary>
    [Required]
    public CompanyRequest Company { get; set; } = null!;
}
