using Fenicia.Common;

namespace Fenicia.Auth.Domains.Order;

public interface IOrderService
{
    Task<ApiResponse<OrderResponse>> CreateNewOrderAsync(
        Guid userId,
        Guid companyId,
        OrderRequest request
    );
}
