using Fenicia.Common.Data.Abstracts;
using Fenicia.Common.Data.Models.Basic;

namespace Fenicia.Module.Basic.Domains.OrderDetail;

public interface IOrderDetailRepository : IBaseRepository<OrderDetailModel>
{
    Task<List<OrderDetailModel>> GetByOrderIdAsync(Guid orderId, CancellationToken ct);
}