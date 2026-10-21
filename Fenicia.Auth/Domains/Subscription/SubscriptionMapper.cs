using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.DTOs.Auth.Subscription;

namespace Fenicia.Auth.Domains.Subscription;

/// <summary>
/// Mapper class for mapping between SubscriptionModel and SubscriptionResponse/UserSubscriptionResponse.
/// </summary>
public static class SubscriptionMapper
{
    /// <summary>
    /// Maps a SubscriptionModel to a SubscriptionResponse.
    /// </summary>
    /// <param name="subscription">The subscription model.</param>
    /// <returns>The subscription response.</returns>
    public static SubscriptionResponse MapToSubscriptionResponse(SubscriptionModel subscription)
    {
        return new SubscriptionResponse(
            subscription.UserId,
            subscription.CompanyId,
            subscription.StartDate,
            subscription.EndDate,
            subscription.Status);
    }

    /// <summary>
    /// Maps a UserSubscriptionResponse to a SubscriptionModel.
    /// </summary>
    /// <param name="response">The user subscription response.</param>
    /// <returns>The subscription model.</returns>
    public static SubscriptionModel MapToSubscriptionModel(UserSubscriptionResponse response)
    {
        return new SubscriptionModel
        {
            UserId = response.UserId,
            CompanyId = response.CompanyId,
            StartDate = response.StartDate,
            EndDate = response.EndDate,
            Status = response.Status
        };
    }

    /// <summary>
    /// Maps a SubscriptionCreditModel to a UserSubscriptionResponse.
    /// </summary>
    /// <param name="credit">The subscription credit model.</param>
    /// <returns>The user subscription response.</returns>
    public static UserSubscriptionResponse MapToUserSubscriptionResponse(SubscriptionCreditModel credit)
    {
        return new UserSubscriptionResponse(
            credit.Id,
            credit.CompanyId,
            credit.CompanyName,
            credit.Status,
            credit.StartDate,
            credit.EndDate,
            credit.CreditsUsed,
            credit.CreditsTotal);
    }
}
