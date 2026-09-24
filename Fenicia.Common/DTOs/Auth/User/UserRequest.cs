using System.ComponentModel.DataAnnotations;
using Fenicia.Common.DTOs.Auth.Company;
using Fenicia.Common.DTOs.Auth.Role;

namespace Fenicia.Common.DTOs.Auth.User;

/// <summary>
/// Request DTO for creating or updating a user.
/// </summary>
public class UserRequest()
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UserRequest"/> class with the specified user details.
    /// </summary>
    /// <param name="email">The user's email address.</param>
    /// <param name="password">The user's password.</param>
    /// <param name="name">The user's name.</param>
    /// <param name="company">The company information for the user.</param>
    /// <param name="roles">The optional list of role assignments for the user.</param>
    public UserRequest(string email, string password, string name, CompanyRequest company, List<RoleRequest>? roles = null)
        : this()
    {
        Email = email;
        Password = password;
        Name = name;
        Roles = roles;
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
    /// Gets or sets the optional list of role assignments for the user.
    /// </summary>
    public List<RoleRequest>? Roles { get; set; }

    /// <summary>
    /// Gets or sets the unique identifier of the user (used for updates).
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the company information for the user.
    /// </summary>
    public CompanyRequest Company { get; set; } = null!;
}
