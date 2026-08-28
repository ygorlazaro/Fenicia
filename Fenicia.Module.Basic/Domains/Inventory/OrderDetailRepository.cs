using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Basic;
using Fenicia.Common.Data.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Domains.Inventory;

public class OrderDetailRepository(DefaultContext context) : Repository<OrderDetailModel>(context)
{
    public async Task<IEnumerable<OrderDetailModel>> GetByDateRangeAsync(DateTime startDate, CancellationToken ct = default)
    {
        return await DbSet
                .Where(d => d.Order.SaleDate >= startDate && d.Deleted == null)
            .Include(d => d.Order)
            .ToListAsync(ct);
    }
}
