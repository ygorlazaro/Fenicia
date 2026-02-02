using Fenicia.Common;
using Fenicia.Common.Database.Requests;
using Fenicia.Common.Database.Responses;

namespace Fenicia.Auth.Domains.Order;

public interface IOrderService
{
    Task<ApiResponse<OrderResponse>> CreateNewOrderAsync(Guid userId, Guid companyId, OrderRequest request, CancellationToken cancellationToken);
}
