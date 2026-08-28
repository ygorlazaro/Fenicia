using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Basic;
using Fenicia.Common.Data.Repositories;
using Fenicia.Module.Basic.Domains.Inventory.DTOs;
using Fenicia.Module.Basic.Domains.Supplier.DTOs;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Domains.Supplier;

public class SupplierRepository(DefaultContext context) : Repository<SupplierModel>(context)
{
    private readonly DefaultContext _context = context;

    public DefaultContext Context => _context;

    public async Task<List<SupplierModel>> GetAllWithDetailsAsync(int page = 1, int perPage = 10, CancellationToken ct = default)
    {
        return await DbSet
                .Where(e => e.Deleted == null)
            .Include(s => s.Person)
            .Include(s => s.Person.PersonAddresses)
                .ThenInclude(pa => pa.Address)
                    .ThenInclude(a => a.State)
            .Skip((page - 1) * perPage)
            .Take(perPage)
            .ToListAsync(ct);
    }

    public async Task<SupplierModel?> GetByIdWithDetailsAsync(Guid id, CancellationToken ct = default)
    {
        return await DbSet
                .Where(e => e.Deleted == null)
            .Include(s => s.Person)
            .Include(s => s.Person.PersonAddresses)
                .ThenInclude(pa => pa.Address)
                    .ThenInclude(a => a.State)
            .FirstOrDefaultAsync(e => e.Id == id, ct);
    }

    public async Task<List<SupplierProductCountResponse>> GetProductStatsAsync(CancellationToken ct = default)
    {
        return await _context.BasicProducts
                .Where(p => p.SupplierId.HasValue && p.Deleted == null)
            .GroupBy(p => p.SupplierId!.Value)
            .Select(g => new SupplierProductCountResponse(
                g.Key,
                string.Empty,
                g.Count(),
                g.Sum(p => (p.CostPrice ?? 0m) * (decimal)p.Quantity),
                g.Sum(p => p.SalesPrice * (decimal)p.Quantity)))
            .ToListAsync(ct);
    }

    public async Task<Dictionary<Guid, string>> GetSupplierNamesAsync(IEnumerable<Guid> supplierIds, CancellationToken ct = default)
    {
        var ids = supplierIds.ToList();
        return await DbSet
            .Where(s => ids.Contains(s.Id) && s.Deleted == null)
            .Include(s => s.Person)
            .ToDictionaryAsync(s => s.Id, s => s.Person.Name, ct);
    }

    public async Task<List<SupplierStockMovementResponse>> GetRecentStockMovementsAsync(int days, int topLimit, CancellationToken ct = default)
    {
        var startDate = DateTime.UtcNow.AddDays(-days);
        return await _context.BasicStockMovements
            .Include(m => m.Product)
            .Where(m => m.SupplierId.HasValue && m.Date >= startDate && m.Deleted == null)
            .OrderByDescending(m => m.Date)
            .Take(topLimit)
            .Select(m => new SupplierStockMovementResponse(
                m.Id,
                m.ProductId,
                m.Product.Name,
                m.Quantity,
                m.Price ?? 0,
                m.Date!.Value,
                m.Type.ToString()))
            .ToListAsync(ct);
    }

    public async Task<List<SupplierCostComparisonResponse>> GetCostComparisonAsync(int topLimit, CancellationToken ct = default)
    {
        return await _context.BasicProducts
                .Include(p => p.Supplier)
            .Where(p => p.SupplierId.HasValue && p.Deleted == null)
            .GroupBy(p => p.Name)
            .Where(g => g.Count() > 1)
            .Select(g => new SupplierCostComparisonResponse(
                g.Key,
                g.Select(p => new ProductSupplierPriceResponse(
                    p.SupplierId!.Value,
                    p.Supplier!.Person.Name,
                    p.CostPrice ?? 0,
                    p.SalesPrice,
                    p.SalesPrice > 0 ? (p.SalesPrice - (p.CostPrice ?? 0)) / p.SalesPrice * 100 : 0)).ToList()))
            .Take(topLimit)
            .ToListAsync(ct);
    }

    public async Task<List<SupplierBreakdownResponse>> GetSupplierBreakdownAsync(CancellationToken ct = default)
    {
        var suppliers = await DbSet
                .Where(s => s.Deleted == null)
            .Include(s => s.Person)
            .ToListAsync(ct);

        var products = await _context.BasicProducts
            .Where(p => p.SupplierId.HasValue && p.Deleted == null)
            .Include(p => p.Supplier)
            .ThenInclude(s => s!.Person)
            .ToListAsync(ct);

        var result = products
            .GroupBy(p => new { SupplierId = p.SupplierId!.Value, SupplierName = p.Supplier!.Person.Name })
            .Select(g => new SupplierBreakdownResponse(
                g.Key.SupplierId,
                g.Key.SupplierName,
                g.Sum(p => (p.CostPrice ?? 0) * (decimal)p.Quantity),
                g.Sum(p => p.SalesPrice * (decimal)p.Quantity),
                g.Sum(p => p.Quantity)))
            .OrderByDescending(s => s.TotalSalesValue)
            .ToList();

        return result;
    }
}
