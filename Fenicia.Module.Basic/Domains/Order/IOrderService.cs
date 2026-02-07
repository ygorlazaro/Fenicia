using Fenicia.Common.Data.Requests.Basic;
using Fenicia.Common.Data.Responses.Basic;

namespace Fenicia.Module.Basic.Domains.Order;

public interface IOrderService
{
    Task<OrderResponse?> AddAsync(OrderRequest orderRequest, CancellationToken ct);
}
