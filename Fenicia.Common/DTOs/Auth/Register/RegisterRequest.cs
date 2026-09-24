using System.ComponentModel.DataAnnotations;
using Fenicia.Common.DTOs.Auth.Company;

namespace Fenicia.Common.DTOs.Auth.Register;

/// <summary>
/// Request DTO for user registration containing email, password, name, and company information.
/// </summary>
public class RegisterRequest()
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RegisterRequest"/> class with the specified details.
    /// </summary>
    /// <param name="email">The user's email address.</param>
    /// <param name="password">The user's password.</param>
    /// <param name="name">The user's name.</param>
    /// <param name="company">The company information for the new user.</param>
    public RegisterRequest(string email, string password, string name, CompanyRequest company)
        : this()
    {
        Email = email;
        Password = password;
        Name = name;
        Company = company;
    }

    /// <summary>
    /// Gets or sets the user's email address.
    /// </summary>
    [Required]
    [EmailAddress]
    [StringLength(48)]
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the user's password.
    /// </summary>
    [Required]
    [StringLength(200)]
    public string Password { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the user's name.
    /// </summary>
    [Required]
    [StringLength(48)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the company information for the new user.
    /// </summary>
    [Required]
    public CompanyRequest Company { get; set; } = new();
}
