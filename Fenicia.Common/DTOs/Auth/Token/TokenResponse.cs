using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.Auth.Token;

/// <summary>
/// Response DTO containing the generated JWT token and user information.
/// </summary>
public class TokenResponse()
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TokenResponse"/> class with the specified user details and roles.
    /// </summary>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <param name="name">The user's name.</param>
    /// <param name="email">The user's email address.</param>
    /// <param name="companyId">The unique identifier of the user's company.</param>
    /// <param name="roles">The list of roles assigned to the user.</param>
    public TokenResponse(Guid userId, string name, string email, Guid companyId, List<string> roles)
        : this()
    {
        UserId = userId;
        Name = name;
        Email = email;
        CompanyId = companyId;
        Roles = roles;
    }

    /// <summary>
    /// Gets or sets the unique identifier of the user.
    /// </summary>
    [Required]
    public Guid UserId { get; set; }

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
    /// Gets or sets the unique identifier of the user's company.
    /// </summary>
    public Guid CompanyId { get; set; }

    /// <summary>
    /// Gets the list of roles assigned to the user.
    /// </summary>
    public List<string> Roles { get; } = [];

    /// <summary>
    /// Gets or sets the JWT access token.
    /// </summary>
    public string Token { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the refresh token.
    /// </summary>
    public string RefreshToken { get; set; } = string.Empty;
}
