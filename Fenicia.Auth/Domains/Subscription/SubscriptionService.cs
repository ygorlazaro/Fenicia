using Fenicia.Common;
using Fenicia.Common.Data.Mappers.Auth;
using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.Data.Responses.Auth;
using Fenicia.Common.Enums.Auth;

namespace Fenicia.Auth.Domains.Subscription;

public sealed class SubscriptionService(ISubscriptionRepository subscriptionRepository) : ISubscriptionService
{
    public async Task<SubscriptionResponse> CreateCreditsForOrderAsync(
        OrderModel order,
        List<OrderDetailModel> details,
        Guid companyId,
        CancellationToken ct)
    {
        if (details.Count == 0) throw new ArgumentException(TextConstants.ThereWasAnErrorAddingModulesMessage);

        var credits = order.Details.Select(d => new SubscriptionCreditModel
        {
            ModuleId = d.ModuleId,
            IsActive = true,
            StartDate = DateTime.UtcNow,
            EndDate = DateTime.UtcNow.AddMonths(1),
            OrderDetailId = d.Id
        }).ToList();

        var subscription = new SubscriptionModel
        {
            Status = SubscriptionStatus.Active,
            CompanyId = companyId,
            StartDate = DateTime.UtcNow,
            EndDate = DateTime.UtcNow.AddMonths(1),
            OrderId = order.Id,
            Credits = credits
        };

        subscriptionRepository.Add(subscription);

        await subscriptionRepository.SaveChangesAsync(ct);

        return SubscriptionMapper.Map(subscription);
    }

    public async Task<List<Guid>> GetValidSubscriptionsAsync(Guid companyId, CancellationToken ct)
    {
        return await subscriptionRepository.GetValidSubscriptionAsync(companyId, ct);
    }
}