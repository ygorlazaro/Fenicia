using Fenicia.Common.Data;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Basic;
using Fenicia.Common.Data.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Domains.Inventory;

public class StockMovementRepository(DefaultContext context) : Repository<StockMovementModel>(context)
{
    public async Task<IEnumerable<StockMovementModel>> GetByDateRangeAsync(DateTime startDate, CancellationToken ct = default)
    {
        return await DbSet
            .Where(m => m.Date >= startDate && m.Deleted == null)
            .ToListAsync(ct);
    }

    public async Task<Dictionary<Guid, DateTime?>> GetLastMovementsByProductIdsAsync(IEnumerable<Guid> productIds, CancellationToken ct = default)
    {
        var ids = productIds.ToList();
        return await DbSet
            .Where(m => ids.Contains(m.ProductId) && m.Deleted == null)
            .GroupBy(m => m.ProductId)
            .Select(g => new
            {
                ProductId = g.Key,
                LastDate = g.OrderByDescending(m => m.Date).Select(m => m.Date).FirstOrDefault()
            })
            .ToDictionaryAsync(k => k.ProductId, v => v.LastDate, ct);
    }
}
