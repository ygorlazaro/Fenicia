using System.ComponentModel.DataAnnotations;
using Fenicia.Common.DTOs.Auth.UserRole;

namespace Fenicia.Common.DTOs.Auth.Subscription;

/// <summary>
/// Response DTO containing user profile with companies and subscriptions.
/// </summary>
public class SubscriptionResponse()
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SubscriptionResponse"/> class with the specified user profile details.
    /// </summary>
    /// <param name="id">The unique identifier of the user.</param>
    /// <param name="name">The user's name.</param>
    /// <param name="email">The user's email address.</param>
    /// <param name="companies">The list of companies associated with the user.</param>
    /// <param name="subscriptions">The list of subscriptions for the user.</param>
    public SubscriptionResponse(
        Guid id,
        string name,
        string email,
        IEnumerable<UserCompanyResponse> companies,
        IEnumerable<UserSubscriptionResponse> subscriptions)
        : this()
    {
        Id = id;
        Name = name;
        Email = email;
        Companies = companies;
        Subscriptions = subscriptions;
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
    /// Gets or sets the list of companies associated with the user.
    /// </summary>
    [Required]
    public IEnumerable<UserCompanyResponse> Companies { get; set; } = [];

    /// <summary>
    /// Gets or sets the list of subscriptions for the user.
    /// </summary>
    [Required]
    public IEnumerable<UserSubscriptionResponse> Subscriptions { get; set; } = [];
}
