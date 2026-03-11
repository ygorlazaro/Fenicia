using Fenicia.Auth.Domains.Subscription.Commands;
using Fenicia.Auth.Domains.Subscription.Responses;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.Enums.Auth;
using Fenicia.Common.Exceptions;
using Fenicia.Common.Localization;

namespace Fenicia.Auth.Domains.Subscription.Handlers;

public class CreateCreditsForOrderHandler(DefaultContext db)
{
    public virtual async Task<CreateCreditsForOrderResponse> Handle(
        CreateCreditsForOrderCommand command,
        CancellationToken ct)
    {
        if (!command.Details.Any())
        {
            throw new InvalidRequestException(ExceptionMessages.OrderDetailsCannotBeEmpty);
        }

        var credits = command.Details.Select(d => new SubscriptionCreditModel
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
            CompanyId = command.CompanyId,
            StartDate = DateTime.UtcNow,
            EndDate = DateTime.UtcNow.AddMonths(1),
            OrderId = command.Id,
            Credits = credits
        };

        db.AuthSubscriptions.Add(subscription);

        await db.SaveChangesAsync(ct);

        return new CreateCreditsForOrderResponse(subscription.Id, subscription.CompanyId, subscription.StartDate,
            subscription.EndDate, command.Id, subscription.Status);
    }
}
