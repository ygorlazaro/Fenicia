using Fenicia.Common.Database.Responses.Basic;

namespace Fenicia.Module.Basic.Domains.OrderDetail;

public interface IOrderDetailService
{
    Task<List<OrderDetailResponse>> GetByOrderIdAsync(Guid orderId, CancellationToken cancellationToken);
}
