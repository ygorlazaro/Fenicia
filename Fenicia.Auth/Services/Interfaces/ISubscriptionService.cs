using Fenicia.Auth.Contexts.Models;
using Fenicia.Auth.Responses;
using Fenicia.Common;

namespace Fenicia.Auth.Services.Interfaces;

public interface ISubscriptionService
{
    Task<ApiResponse<SubscriptionResponse>> CreateCreditsForOrderAsync(
        OrderModel order,
        List<OrderDetailModel> details,
        Guid companyId
    );
    Task<ApiResponse<List<Guid>>> GetValidSubscriptionsAsync(Guid companyId);
}
