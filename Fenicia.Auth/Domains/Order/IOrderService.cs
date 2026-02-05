using Fenicia.Common.Data.Requests.Auth;
using Fenicia.Common.Data.Responses.Auth;

namespace Fenicia.Auth.Domains.Order;

public interface IOrderService
{
    Task<OrderResponse?> CreateNewOrderAsync(Guid userId, Guid companyId, OrderRequest request, CancellationToken cancellationToken);
}
