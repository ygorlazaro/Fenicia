using Fenicia.Common.Data.Models.Basic;
using Fenicia.Common.Data.Repositories;
using Fenicia.Module.Basic.Domains.OrderDetail.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Domains.OrderDetail;

/// <summary>
/// Repository implementation for managing order detail data.
/// </summary>
/// <param name="context">The database context.</param>
public class OrderDetailRepository(DbContext context)
    : Repository<OrderDetailModel>(context), IOrderDetailRepository
{
    /// <inheritdoc />
    public async Task<IEnumerable<OrderDetailModel>> GetByOrderIdAsync(
        Guid orderId,
        CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(d => d.Product)
            .Where(d => d.OrderId == orderId)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public Task<Dictionary<Guid, int>> GetDetailCountsByOrderIdsAsync(
        IEnumerable<Guid> orderIds,
        CancellationToken cancellationToken = default)
    {
        var ids = orderIds.ToList();
        return DbSet
            .Where(d => ids.Contains(d.OrderId))
            .GroupBy(d => d.OrderId)
            .Select(g => new { OrderId = g.Key, Count = g.Count() })
            .ToDictionaryAsync(k => k.OrderId, v => v.Count, cancellationToken);
    }

    /// <inheritdoc />
    public Task<Dictionary<Guid, double>> GetQuantitySumsByOrderIdsAsync(
        IEnumerable<Guid> orderIds,
        CancellationToken cancellationToken = default)
    {
        var ids = orderIds.ToList();
        return DbSet
            .Where(d => ids.Contains(d.OrderId))
            .GroupBy(d => d.OrderId)
            .Select(g => new { OrderId = g.Key, Qty = g.Sum(d => d.Quantity) })
            .ToDictionaryAsync(k => k.OrderId, v => v.Qty, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IEnumerable<OrderDetailModel>> GetByOrderDateRangeAsync(
        DateTime startDate,
        DateTime endDate,
        CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(d => d.Order)
            .Where(d => d.Order.SaleDate >= startDate && d.Order.SaleDate <= endDate)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IEnumerable<OrderDetailModel>> GetByDateRangeAsync(
        DateTime startDate,
        CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Where(d => d.Order.SaleDate >= startDate)
            .Include(d => d.Order)
            .ToListAsync(cancellationToken);
    }
}