using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Basic;
using Fenicia.Common.Data.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Domains.OrderDetail;

public class OrderDetailRepository(DefaultContext context) : Repository<OrderDetailModel>(context)
{
    public async Task<IEnumerable<OrderDetailModel>> GetByOrderIdAsync(Guid orderId, CancellationToken ct = default)
    {
        return await DbSet
                .Include(d => d.Product)
                .Where(d => d.OrderId == orderId && d.Deleted == null)
            .ToListAsync(ct);
    }

    public async Task<Dictionary<Guid, int>> GetDetailCountsByOrderIdsAsync(IEnumerable<Guid> orderIds, CancellationToken ct = default)
    {
        var ids = orderIds.ToList();
        return await DbSet
            .Where(d => ids.Contains(d.OrderId) && d.Deleted == null)
            .GroupBy(d => d.OrderId)
            .Select(g => new { OrderId = g.Key, Count = g.Count() })
            .ToDictionaryAsync(k => k.OrderId, v => v.Count, ct);
    }

    public async Task<Dictionary<Guid, double>> GetQuantitySumsByOrderIdsAsync(IEnumerable<Guid> orderIds, CancellationToken ct = default)
    {
        var ids = orderIds.ToList();
        return await DbSet
            .Where(d => ids.Contains(d.OrderId) && d.Deleted == null)
            .GroupBy(d => d.OrderId)
            .Select(g => new { OrderId = g.Key, Qty = g.Sum(d => d.Quantity) })
            .ToDictionaryAsync(k => k.OrderId, v => v.Qty, ct);
    }

    public async Task<IEnumerable<OrderDetailModel>> GetByOrderDateRangeAsync(DateTime startDate, DateTime endDate, CancellationToken ct = default)
    {
        return await DbSet
                .Include(d => d.Order)
            .Where(d => d.Order.SaleDate >= startDate && d.Order.SaleDate <= endDate && d.Deleted == null)
            .ToListAsync(ct);
    }

    public async Task<IEnumerable<OrderDetailModel>> GetByDateRangeAsync(DateTime startDate, CancellationToken ct = default)
    {
        return await DbSet
                .Where(d => d.Order.SaleDate >= startDate && d.Deleted == null)
            .Include(d => d.Order)
            .ToListAsync(ct);
    }
}
