using Fenicia.Common.Database.Requests.Auth;
using Fenicia.Common.Database.Responses.Auth;

namespace Fenicia.Auth.Domains.Order;

public interface IOrderService
{
    Task<OrderResponse?> CreateNewOrderAsync(Guid userId, Guid companyId, OrderRequest request, CancellationToken cancellationToken);
}
