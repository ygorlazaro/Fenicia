using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Basic;
using Fenicia.Common.Data.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Domains.StockMovement;

public class StockMovementRepository(DefaultContext context)
    : Repository<StockMovementModel>(context), IStockMovementRepository
{
    public async Task<IEnumerable<StockMovementModel>> GetByDateRangeAsync(
        DateTime startDate,
        DateTime endDate,
        CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Where(m => m.Date >= startDate && m.Date <= endDate)
            .ToListAsync(cancellationToken);
    }

    public Task<Dictionary<Guid, DateTime?>> GetLastMovementsByProductIdsAsync(
        IEnumerable<Guid> productIds,
        CancellationToken cancellationToken = default)
    {
        var ids = productIds.ToList();
        return DbSet
            .Where(m => ids.Contains(m.ProductId))
            .GroupBy(m => m.ProductId)
            .Select(g => new
            {
                ProductId = g.Key,
                LastDate = g.OrderByDescending(m => m.Date).Select(m => m.Date).FirstOrDefault()
            })
            .ToDictionaryAsync(k => k.ProductId, v => v.LastDate, cancellationToken);
    }

    public async Task<IEnumerable<StockMovementModel>> GetWithDetailsAsync(
        DateTime startDate,
        DateTime endDate,
        int page = 1,
        int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Where(m => m.Date >= startDate && m.Date <= endDate)
            .Include(m => m.Product).ThenInclude(p => p.Category)
            .Include(m => m.Customer!).ThenInclude(c => c.Person)
            .Include(m => m.Supplier!).ThenInclude(s => s.Person)
            .Include(m => m.Employee!).ThenInclude(e => e.Person)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<StockMovementModel>> GetWithDetailsForDashboardAsync(
        DateTime startDate,
        DateTime endDate,
        CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Where(m => m.Date >= startDate && m.Date <= endDate)
            .Include(m => m.Product).ThenInclude(p => p.Category)
            .Include(m => m.Customer!).ThenInclude(c => c.Person)
            .Include(m => m.Supplier!).ThenInclude(s => s.Person)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<StockMovementModel>> GetByDateRangeAsync(
        DateTime startDate,
        CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Where(m => m.Date >= startDate)
            .ToListAsync(cancellationToken);
    }
}