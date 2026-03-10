using Fenicia.Common.Data.Contexts;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Domains.Inventory.GetInventoryDashboard;

public class GetInventoryDashboardHandler(DefaultContext context)
{
    public async Task<InventoryDashboardResponse> Handle(CancellationToken ct)
    {
        var products = await context.BasicProducts
            .Include(p => p.Category)
            .Include(p => p.Supplier)
            .ThenInclude(s => s != null ? s.Person : null)
            .ToListAsync(ct);

        var lowStockItems = products
            .OrderBy(p => p.Quantity)
            .Take(5)
            .Select(p => new InventoryDashboardItemResponse(
                p.Id,
                p.Name,
                p.Quantity,
                p.CostPrice,
                p.SalesPrice,
                p.CategoryId,
                p.Category.Name))
            .ToList();

        var totalCustomers = await context.BasicCustomers.CountAsync(ct);
        var totalEmployees = await context.BasicEmployees.CountAsync(ct);

        var totalCostValue = products.Sum(p => (p.CostPrice ?? 0) * (decimal)p.Quantity);
        var totalSalesValue = products.Sum(p => p.SalesPrice * (decimal)p.Quantity);
        var totalQuantity = products.Sum(p => p.Quantity);
        var profitPotential = totalSalesValue - totalCostValue;

        var categoryBreakdown = products
            .GroupBy(p => new { p.CategoryId, CategoryName = p.Category.Name })
            .Select(g => new CategoryBreakdownResponse(
                g.Key.CategoryId,
                g.Key.CategoryName,
                g.Sum(p => (p.CostPrice ?? 0) * (decimal)p.Quantity),
                g.Sum(p => p.SalesPrice * (decimal)p.Quantity),
                g.Sum(p => p.Quantity)))
            .OrderByDescending(c => c.TotalSalesValue)
            .ToList();

        var supplierBreakdown = products
            .Where(p => p.SupplierId.HasValue)
            .GroupBy(p => new { SupplierId = p.SupplierId!.Value, SupplierName = p.Supplier!.Person.Name })
            .Select(g => new SupplierBreakdownResponse(
                g.Key.SupplierId,
                g.Key.SupplierName,
                g.Sum(p => (p.CostPrice ?? 0) * (decimal) p.Quantity),
                g.Sum(p => p.SalesPrice * (decimal)p.Quantity),
                g.Sum(p => p.Quantity)))
            .OrderByDescending(s => s.TotalSalesValue)
            .ToList();

        return new InventoryDashboardResponse
        {
            LowStockItems = lowStockItems,
            TotalCustomers = totalCustomers,
            TotalEmployees = totalEmployees,
            TotalCostValue = totalCostValue,
            TotalSalesValue = totalSalesValue,
            TotalQuantity = totalQuantity,
            ProfitPotential = profitPotential,
            CategoryBreakdown = categoryBreakdown,
            SupplierBreakdown = supplierBreakdown
        };
    }
}