using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Basic;
using Fenicia.Common.Data.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Domains.OrderDetail;

public class OrderDetailRepository(DefaultContext context) : Repository<OrderDetailModel>(context), IOrderDetailRepository
{
    public async Task<IEnumerable<OrderDetailModel>> GetByOrderIdAsync(Guid orderId, CancellationToken cancellationToken = default)
    {
        return await DbSet
                .Include(d => d.Product)
                .Where(d => d.OrderId == orderId)
            .ToListAsync(cancellationToken);
    }

    public async Task<Dictionary<Guid, int>> GetDetailCountsByOrderIdsAsync(IEnumerable<Guid> orderIds, CancellationToken cancellationToken = default)
    {
        var ids = orderIds.ToList();
        return await DbSet
            .Where(d => ids.Contains(d.OrderId))
            .GroupBy(d => d.OrderId)
            .Select(g => new { OrderId = g.Key, Count = g.Count() })
            .ToDictionaryAsync(k => k.OrderId, v => v.Count, cancellationToken);
    }

    public async Task<Dictionary<Guid, double>> GetQuantitySumsByOrderIdsAsync(IEnumerable<Guid> orderIds, CancellationToken cancellationToken = default)
    {
        var ids = orderIds.ToList();
        return await DbSet
            .Where(d => ids.Contains(d.OrderId))
            .GroupBy(d => d.OrderId)
            .Select(g => new { OrderId = g.Key, Qty = g.Sum(d => d.Quantity) })
            .ToDictionaryAsync(k => k.OrderId, v => v.Qty, cancellationToken);
    }

    public async Task<IEnumerable<OrderDetailModel>> GetByOrderDateRangeAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default)
    {
        return await DbSet
                .Include(d => d.Order)
            .Where(d => d.Order.SaleDate >= startDate && d.Order.SaleDate <= endDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<OrderDetailModel>> GetByDateRangeAsync(DateTime startDate, CancellationToken cancellationToken = default)
    {
        return await DbSet
                .Where(d => d.Order.SaleDate >= startDate)
            .Include(d => d.Order)
            .ToListAsync(cancellationToken);
    }
}
