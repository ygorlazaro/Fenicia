using Fenicia.Common.Database.Abstracts;
using Fenicia.Common.Database.Contexts;
using Fenicia.Common.Database.Models.Basic;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Domains.OrderDetail;

public class OrderDetailRepository(BasicContext context) : BaseRepository<OrderDetailModel>(context), IOrderDetailRepository
{
    public async Task<List<OrderDetailModel>> GetByOrderIdAsync(Guid orderId, CancellationToken cancellationToken)
    {
        return await context.OrderDetails.Where(od => od.OrderId == orderId).ToListAsync(cancellationToken);
    }
}
