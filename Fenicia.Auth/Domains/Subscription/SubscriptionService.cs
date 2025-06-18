using System.Net;

using AutoMapper;

using Fenicia.Auth.Domains.Order;
using Fenicia.Auth.Domains.OrderDetail;
using Fenicia.Auth.Domains.SubscriptionCredit;
using Fenicia.Auth.Enums;
using Fenicia.Common;

namespace Fenicia.Auth.Domains.Subscription;

public class SubscriptionService(
    IMapper mapper,
    ILogger<SubscriptionService> logger,
    ISubscriptionRepository subscriptionRepository
) : ISubscriptionService
{
    public async Task<ApiResponse<SubscriptionResponse>> CreateCreditsForOrderAsync(
        OrderModel order,
        List<OrderDetailModel> details,
        Guid companyId
    )
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
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddMonths(1),
                OrderDetailId = d.Id,
            })
            .ToList();

        var subscription = new SubscriptionModel
        {
            Status = SubscriptionStatus.Active,
            CompanyId = companyId,
            StartDate = DateTime.Now,
            EndDate = DateTime.Now.AddMonths(1),
            OrderId = order.Id,
            Credits = credits,
        };

        await subscriptionRepository.SaveSubscription(subscription);

        var response = mapper.Map<SubscriptionResponse>(subscription);

        return new ApiResponse<SubscriptionResponse>(response);
    }

    public async Task<ApiResponse<List<Guid>>> GetValidSubscriptionsAsync(Guid companyId)
    {
        logger.LogInformation("Getting valid subscriptions");
        var response = await subscriptionRepository.GetValidSubscriptionAsync(companyId);

        return new ApiResponse<List<Guid>>(response);
    }
}
