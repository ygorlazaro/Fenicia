namespace Fenicia.Auth.Domains.Order.Logic;

using Common;

using Data;

public interface IOrderService
{
    Task<ApiResponse<OrderResponse>> CreateNewOrderAsync(Guid userId, Guid companyId, OrderRequest request, CancellationToken cancellationToken);
}
