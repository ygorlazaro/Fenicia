using Fenicia.Common.Database.Abstracts;
using Fenicia.Common.Database.Models.Basic;

namespace Fenicia.Module.Basic.Domains.OrderDetail;

public interface IOrderDetailRepository : IBaseRepository<OrderDetailModel>
{
    Task<List<OrderDetailModel>> GetByOrderIdAsync(Guid orderId, CancellationToken cancellationToken);
}
