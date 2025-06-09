using Fenicia.Auth.Contexts.Models;
using Fenicia.Auth.Responses;
using Fenicia.Common;

namespace Fenicia.Auth.Services.Interfaces;

public interface ISubscriptionService
{
    Task<ServiceResponse<SubscriptionResponse>> CreateCreditsForOrderAsync(
        OrderModel order,
        List<OrderDetailModel> details,
        Guid companyId
    );
    Task<ServiceResponse<List<Guid>>> GetValidSubscriptionsAsync(Guid companyId);
}
