using System.ComponentModel.DataAnnotations;
using Fenicia.Common.DTOs.Auth.UserRole;

namespace Fenicia.Common.DTOs.Auth.User;

/// <summary>
/// Response DTO containing user details with associated company information.
/// </summary>
public class UserResponse()
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UserResponse"/> class with the specified user details.
    /// </summary>
    /// <param name="id">The unique identifier of the user.</param>
    /// <param name="name">The user's name.</param>
    /// <param name="email">The user's email address.</param>
    /// <param name="companyResponse">The company information associated with the user.</param>
    public UserResponse(Guid id, string name, string email, CompanyResponse companyResponse)
        : this()
    {
        Id = id;
        Name = name;
        Email = email;
        Company = companyResponse;
    }

    /// <summary>
    /// Gets or sets the unique identifier of the user.
    /// </summary>
    [Required]
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the user's name.
    /// </summary>
    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the user's email address.
    /// </summary>
    [Required]
    [MaxLength(200)]
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the company information associated with the user.
    /// </summary>
    public CompanyResponse Company { get; set; } = null!;
}
