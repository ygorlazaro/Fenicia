using AutoMapper;
using Fenicia.Auth.Contexts.Models;
using Fenicia.Auth.Enums;
using Fenicia.Auth.Repositories.Interfaces;
using Fenicia.Auth.Responses;
using Fenicia.Auth.Services.Interfaces;
using Fenicia.Common;

namespace Fenicia.Auth.Services;

public class SubscriptionService(IMapper mapper, ISubscriptionRepository subscriptionRepository) : ISubscriptionService
{
    public async Task<SubscriptionResponse?> CreateCreditsForOrderAsync(OrderModel order, List<OrderDetailModel> details,
        Guid companyId)
    {
        if (details.Count == 0)
        {
            throw new InvalidDataException(TextConstants.ThereWasAnErrorAddingModules);
        }

        var credits = order.Details.Select(d =>
            new SubscriptionCreditModel
            {
                ModuleId = d.ModuleId,
                IsActive = true,
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddMonths(1),
                OrderDetailId = d.Id
            }
        ).ToList();

        var subscription = new SubscriptionModel
        {
            Status = SubscriptionStatus.Active,
            CompanyId = companyId,
            StartDate = DateTime.Now,
            EndDate = DateTime.Now.AddMonths(1),
            OrderId = order.Id,
            Credits = credits
        };

        await subscriptionRepository.SaveSubscription(subscription);

        return mapper.Map<SubscriptionResponse>(subscription);
    }

    public async Task<List<Guid>> GetValidSubscriptionsAsync(Guid companyId)
    {
        return await subscriptionRepository.GetValidSubscriptionAsync(companyId); 
    }
}