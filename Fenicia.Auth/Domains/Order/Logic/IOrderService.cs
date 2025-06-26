using Fenicia.Auth.Domains.Order.Data;
using Fenicia.Common;

namespace Fenicia.Auth.Domains.Order.Logic;

public interface IOrderService
{
    Task<ApiResponse<OrderResponse>> CreateNewOrderAsync(Guid userId,
        Guid companyId,
        OrderRequest request, CancellationToken cancellationToken);
}
