using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.Auth.Subscription;

public record GetUserProfileResponse(
    [Required] Guid Id,
    [Required] [MaxLength(200)] string Name,
    [Required] [MaxLength(200)] string Email,
    IEnumerable<UserCompanyResponse> Companies,
    IEnumerable<UserSubscriptionResponse> Subscriptions);