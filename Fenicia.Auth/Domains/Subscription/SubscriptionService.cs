using Fenicia.Common;
using Fenicia.Common.Database.Models.Auth;
using Fenicia.Common.Database.Responses;
using Fenicia.Common.Enums;

namespace Fenicia.Auth.Domains.Subscription;

public sealed class SubscriptionService(ISubscriptionRepository subscriptionRepository) : ISubscriptionService
{
    public async Task<SubscriptionResponse> CreateCreditsForOrderAsync(OrderModel order, List<OrderDetailModel> details, Guid companyId, CancellationToken cancellationToken)
    {
        if (details.Count == 0)
        {
            throw new ArgumentException(TextConstants.ThereWasAnErrorAddingModulesMessage);
        }

        var credits = order.Details.Select(d => new SubscriptionCreditModel
        {
            ModuleId = d.ModuleId,
            IsActive = true,
            StartDate = DateTime.UtcNow,
            EndDate = DateTime.UtcNow.AddMonths(months: 1),
            OrderDetailId = d.Id
        }).ToList();

        var subscription = new SubscriptionModel
        {
            Status = SubscriptionStatus.Active,
            CompanyId = companyId,
            StartDate = DateTime.UtcNow,
            EndDate = DateTime.UtcNow.AddMonths(months: 1),
            OrderId = order.Id,
            Credits = credits
        };

        await subscriptionRepository.SaveSubscriptionAsync(subscription, cancellationToken);

        return SubscriptionResponse.Convert(subscription);
    }

    public async Task<List<Guid>> GetValidSubscriptionsAsync(Guid companyId, CancellationToken cancellationToken)
    {
        return await subscriptionRepository.GetValidSubscriptionAsync(companyId, cancellationToken);
    }
}
