using Fenicia.Auth.Contexts.Models;
using Fenicia.Auth.Responses;

namespace Fenicia.Auth.Services.Interfaces;

public interface ISubscriptionService
{
    Task<SubscriptionResponse?> CreateCreditsForOrderAsync(OrderModel order, List<OrderDetailModel> details,
        Guid companyId);
    Task<List<Guid>> GetValidSubscriptionsAsync(Guid companyId);
}