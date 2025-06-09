using System.Net;
using AutoMapper;
using Fenicia.Auth.Contexts.Models;
using Fenicia.Auth.Enums;
using Fenicia.Auth.Repositories.Interfaces;
using Fenicia.Auth.Responses;
using Fenicia.Auth.Services.Interfaces;
using Fenicia.Common;

namespace Fenicia.Auth.Services;

public class SubscriptionService(
    IMapper mapper,
    ILogger<SubscriptionService> logger,
    ISubscriptionRepository subscriptionRepository
) : ISubscriptionService
{
    public async Task<ServiceResponse<SubscriptionResponse>> CreateCreditsForOrderAsync(
        OrderModel order,
        List<OrderDetailModel> details,
        Guid companyId
    )
    {
        logger.LogInformation("Creating credits for order");

        if (details.Count == 0)
        {
            logger.LogWarning("There was an error adding modules");

            return new ServiceResponse<SubscriptionResponse>(
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

        return new ServiceResponse<SubscriptionResponse>(response);
    }

    public async Task<ServiceResponse<List<Guid>>> GetValidSubscriptionsAsync(Guid companyId)
    {
        logger.LogInformation("Getting valid subscriptions");
        var response = await subscriptionRepository.GetValidSubscriptionAsync(companyId);

        return new ServiceResponse<List<Guid>>(response);
    }
}
