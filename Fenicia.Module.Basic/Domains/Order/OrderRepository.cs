using Fenicia.Common.Data;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Basic;
using Fenicia.Common.Data.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Domains.Order;

public class OrderRepository(DefaultContext context) : Repository<OrderModel>(context)
{
    public async Task<OrderModel?> GetByIdWithDetailsAsync(Guid id, CancellationToken ct = default)
    {
        return await DbSet
            .Include(o => o.Customer).ThenInclude(c => c.Person)
            .Include(o => o.Details).ThenInclude(d => d.Product)
            .FirstOrDefaultAsync(e => e.Id == id && e.Deleted == null, ct);
    }

    public async Task<IEnumerable<OrderModel>> GetRecentOrdersAsync(int page = 1, int perPage = 10, CancellationToken ct = default)
    {
        return await DbSet
            .Where(e => e.Deleted == null)
            .OrderByDescending(o => o.SaleDate)
            .Skip((page - 1) * perPage)
            .Take(perPage)
            .ToListAsync(ct);
    }

    public async Task<List<Guid>> GetRecentOrderIdsAsync(int page = 1, int perPage = 10, CancellationToken ct = default)
    {
        return await DbSet
            .Where(e => e.Deleted == null)
            .OrderByDescending(o => o.SaleDate)
            .Skip((page - 1) * perPage)
            .Take(perPage)
            .Select(o => o.Id)
            .ToListAsync(ct);
    }

    public async Task<IEnumerable<OrderModel>> GetAnalyticsOrdersAsync(DateTime startDate, DateTime endDate, CancellationToken ct = default)
    {
        return await DbSet
            .Where(e => e.Deleted == null)
            .Include(o => o.Customer).ThenInclude(c => c.Person)
            .Include(o => o.Details)
            .Where(o => o.SaleDate >= startDate && o.SaleDate <= endDate)
            .ToListAsync(ct);
    }
}
