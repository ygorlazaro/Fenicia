using Fenicia.Common.Data;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Basic;
using Fenicia.Common.Data.Repositories;
using Fenicia.Module.Basic.Domains.Inventory.DTOs;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Domains.Supplier;

public class SupplierRepository(DefaultContext context) : Repository<SupplierModel>(context)
{
    public async Task<List<SupplierBreakdownResponse>> GetSupplierBreakdownAsync(CancellationToken ct = default)
    {
        var suppliers = await DbSet
            .Where(s => s.Deleted == null)
            .Include(s => s.Person)
            .ToListAsync(ct);

        var products = await context.BasicProducts
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
