using Fenicia.Auth.Contexts.Models;

namespace Fenicia.Auth.Services.Interfaces;

public interface ISubscriptionService
{
    Task<SubscriptionModel?> CreateCreditsForOrderAsync(OrderModel order, List<OrderDetailModel> details,
        Guid companyId);
    Task<List<Guid>> GetValidSubscriptionsAsync(Guid companyId);
}