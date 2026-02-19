using Fenicia.Common;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.Enums.Auth;

namespace Fenicia.Auth.Domains.Subscription.CreateCreditsForOrder;

public class CreateCreditsForOrderHandler(AuthContext context)
{
    public async Task<CreateCreditsForOrderResponse> Handle(
        CreateCreditsForOrderQuery order,
        CancellationToken ct)
    {
        if (!order.Details.Any())
        {
            throw new ArgumentException(TextConstants.ThereWasAnErrorAddingModulesMessage);
        }

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
            CompanyId = order.CompanyId,
            StartDate = DateTime.UtcNow,
            EndDate = DateTime.UtcNow.AddMonths(1),
            OrderId = order.Id,
            Credits = credits
        };

        context.Subscriptions.Add(subscription);

        await context.SaveChangesAsync(ct);

        return new CreateCreditsForOrderResponse(subscription.Id, subscription.CompanyId, subscription.StartDate, subscription.EndDate, order.Id, subscription.Status);
    }
}