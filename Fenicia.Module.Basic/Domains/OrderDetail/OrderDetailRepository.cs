using Fenicia.Common.Data.Abstracts;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Basic;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Domains.OrderDetail;

public class OrderDetailRepository(BasicContext context)
    : BaseRepository<OrderDetailModel>(context), IOrderDetailRepository
{
    public async Task<List<OrderDetailModel>> GetByOrderIdAsync(Guid orderId, CancellationToken ct)
    {
        return await context.OrderDetails.Where(od => od.OrderId == orderId).ToListAsync(ct);
    }
}