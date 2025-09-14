namespace Fenicia.Auth.Domains.Order;

using Common;
using Common.Database.Requests;
using Common.Database.Responses;

public interface IOrderService
{
    Task<ApiResponse<OrderResponse>> CreateNewOrderAsync(Guid userId, Guid companyId, OrderRequest request, CancellationToken cancellationToken);
}
