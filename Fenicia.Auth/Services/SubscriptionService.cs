using Fenicia.Auth.Contexts.Models;
using Fenicia.Auth.Enums;
using Fenicia.Auth.Repositories.Interfaces;
using Fenicia.Auth.Services.Interfaces;

namespace Fenicia.Auth.Services;

public class SubscriptionService(ISubscriptionRepository subscriptionRepository) : ISubscriptionService
{
    public async Task<object?> CreateCreditsForOrderAsync(OrderModel order, List<OrderDetailModel> details,
        Guid companyId)
    {
        if (details.Count == 0)
        {
            Console.WriteLine("Ocorreu um problema para adicionar créditos de assinatura");

            return null;
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

        return subscription;
    }
}