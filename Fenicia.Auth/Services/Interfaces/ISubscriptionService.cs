using Fenicia.Auth.Contexts.Models;

namespace Fenicia.Auth.Services.Interfaces;

public interface ISubscriptionService
{
    Task<object?> CreateCreditsForOrderAsync(OrderModel order, List<OrderDetailModel> details, Guid companyId);
}