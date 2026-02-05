using Fenicia.Common.Database.Requests.Basic;
using Fenicia.Common.Database.Responses.Basic;

namespace Fenicia.Module.Basic.Domains.Order;

public interface IOrderService
{
    Task<OrderResponse?> AddAsync(OrderRequest orderRequest, CancellationToken cancellationToken);
}
