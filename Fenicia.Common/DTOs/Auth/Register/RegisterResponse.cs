using System.ComponentModel.DataAnnotations;
using Fenicia.Common.DTOs.Auth.UserRole;

namespace Fenicia.Common.DTOs.Auth.Register;

/// <summary>
/// Response DTO for user registration containing the created user details and associated company.
/// </summary>
public class RegisterResponse()
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RegisterResponse"/> class with the specified user details and company.
    /// </summary>
    /// <param name="id">The unique identifier of the created user.</param>
    /// <param name="name">The user's name.</param>
    /// <param name="email">The user's email address.</param>
    /// <param name="company">The company information associated with the user.</param>
    public RegisterResponse(Guid id, string name, string email, UserCompanyResponse company)
        : this()
    {
        Id = id;
        Name = name;
        Email = email;
        Company = company;
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
    [Required]
    public UserCompanyResponse Company { get; set; } = null!;
}
