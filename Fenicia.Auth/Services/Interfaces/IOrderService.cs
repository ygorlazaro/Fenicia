using Fenicia.Auth.Requests;
using Fenicia.Auth.Responses;
using Fenicia.Common;

namespace Fenicia.Auth.Services.Interfaces;

public interface IOrderService
{
    Task<ApiResponse<OrderResponse>> CreateNewOrderAsync(
        Guid userId,
        Guid companyId,
        OrderRequest request
    );
}
