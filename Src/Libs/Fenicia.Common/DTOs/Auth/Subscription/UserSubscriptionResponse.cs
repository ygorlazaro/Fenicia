using System.ComponentModel.DataAnnotations;
using Fenicia.Common.Enums.Auth;

namespace Fenicia.Common.DTOs.Auth.Subscription;

public record UserSubscriptionResponse(
    [Required] Guid Id,
    [Required] Guid CompanyId,
    [Required] [MaxLength(200)] string CompanyName,
    [Required] SubscriptionStatus Status,
    [Required] DateTime StartDate,
    DateTime? EndDate)
{
    public IEnumerable<UserModuleResponse> Modules { get; set; } = [];
}