using System.Net;

using AutoMapper;

using Fenicia.Auth.Domains.Order.Data;
using Fenicia.Auth.Domains.OrderDetail.Data;
using Fenicia.Auth.Domains.Subscription.Data;
using Fenicia.Auth.Domains.SubscriptionCredit;
using Fenicia.Auth.Domains.SubscriptionCredit.Data;
using Fenicia.Auth.Enums;
using Fenicia.Common;

namespace Fenicia.Auth.Domains.Subscription.Logic;

public class SubscriptionService(
    IMapper mapper,
    ILogger<SubscriptionService> logger,
    ISubscriptionRepository subscriptionRepository
) : ISubscriptionService
{
    public async Task<ApiResponse<SubscriptionResponse>> CreateCreditsForOrderAsync(
        OrderModel order,
        List<OrderDetailModel> details,
        Guid companyId, CancellationToken cancellationToken)
    {
        logger.LogInformation("Creating credits for order");

        if (details.Count == 0)
        {
            logger.LogWarning("There was an error adding modules");

            return new ApiResponse<SubscriptionResponse>(
                null,
                HttpStatusCode.BadRequest,
                TextConstants.ThereWasAnErrorAddingModules
            );
        }

        var credits = order
            .Details.Select(d => new SubscriptionCreditModel
            {
                ModuleId = d.ModuleId,
                IsActive = true,
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow.AddMonths(1),
                OrderDetailId = d.Id
            })
            .ToList();

        var subscription = new SubscriptionModel
        {
            Status = SubscriptionStatus.Active,
            CompanyId = companyId,
            StartDate = DateTime.UtcNow,
            EndDate = DateTime.UtcNow.AddMonths(1),
            OrderId = order.Id,
            Credits = credits
        };

        await subscriptionRepository.SaveSubscription(subscription, cancellationToken);

        var response = mapper.Map<SubscriptionResponse>(subscription);

        return new ApiResponse<SubscriptionResponse>(response);
    }

    public async Task<ApiResponse<List<Guid>>> GetValidSubscriptionsAsync(Guid companyId, CancellationToken cancellationToken)
    {
        logger.LogInformation("Getting valid subscriptions");
        var response = await subscriptionRepository.GetValidSubscriptionAsync(companyId, cancellationToken);

        return new ApiResponse<List<Guid>>(response);
    }
}
