using System.ComponentModel.DataAnnotations;

namespace Fenicia.Auth.Domains.Subscription.DTOs;

public record GetUserProfileResponse(
    [Required] Guid Id,
    [Required] [MaxLength(200)] string Name,
    [Required] [MaxLength(200)] string Email,
    IEnumerable<UserCompanyResponse> Companies,
    IEnumerable<UserSubscriptionResponse> Subscriptions);