using System.ComponentModel.DataAnnotations;
using Fenicia.Common.DTOs.Auth.UserRole;

namespace Fenicia.Common.DTOs.Auth.Subscription;

public class SubscriptionResponse()
{
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

    [Required]
    public Guid Id { get; set; }

    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [MaxLength(200)]
    public string Email { get; set; } = string.Empty;

    [Required]
    public IEnumerable<UserCompanyResponse> Companies { get; set; } = [];

    [Required]
    public IEnumerable<UserSubscriptionResponse> Subscriptions { get; set; } = [];
}
