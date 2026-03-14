using Fenicia.Common.Data.Contexts;
using Fenicia.Module.Basic.Domains.Inventory.Responses;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Domains.Inventory.Handlers;

/// <summary>
/// Handler responsible for generating inventory dashboard data.
/// Provides overview metrics including low stock items, totals, and breakdowns.
/// </summary>
public class GetInventoryDashboardHandler(DefaultContext db)
{
    /// <summary>
    /// Generates inventory dashboard with key metrics and breakdowns.
    /// </summary>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Inventory dashboard with metrics, low stock items, and breakdowns.</returns>
    public async Task<InventoryDashboardResponse> Handle(CancellationToken ct)
    {
        var lowStockItems = await GetInventoryDashboardItemAsync(ct);
        var totalCustomers = await db.BasicCustomers.CountAsync(ct);
        var totalEmployees = await db.BasicEmployees.CountAsync(ct);
        var totalCostValue = await db.BasicProducts.SumAsync(p => (p.CostPrice ?? 0) * (decimal)p.Quantity, ct);
        var totalSalesValue = await db.BasicProducts.SumAsync(p => p.SalesPrice * (decimal)p.Quantity, ct);
        var totalQuantity = await db.BasicProducts.SumAsync(p => p.Quantity, ct);
        var profitPotential = totalSalesValue - totalCostValue;
        var categoryBreakdown = await GetCategoryBreakdownAsync(ct);
        var supplierBreakdown = await GetSupplierBreakdownAsync(ct);

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

        private async Task<List<CategoryBreakdownResponse>> GetCategoryBreakdownAsync(CancellationToken ct)
        {
            var request =
                from p in db.BasicProducts
                group p by new
                {
                    p.CategoryId,
                    CategoryName = p.Category.Name
                }
                into g
                select new CategoryBreakdownResponse(
                    g.Key.CategoryId,
                    g.Key.CategoryName,
                    g.Sum(p => p.CostPrice.Value * (decimal)p.Quantity),
                    g.Sum(p => p.SalesPrice * (decimal)p.Quantity),
                    g.Sum(p => p.Quantity)
                );
            
            return await request.ToListAsync(ct);
        }

    private async Task<List<InventoryDashboardItemResponse>> GetInventoryDashboardItemAsync(CancellationToken ct)
    {
        var lowStockItems = db.BasicProducts
            .OrderBy(p => p.Quantity)
            .Take(5)
            .Select(p => new InventoryDashboardItemResponse(
                p.Id,
                p.Name,
                p.Quantity,
                p.CostPrice,
                p.SalesPrice,
                p.CategoryId,
                p.Category.Name));

        return await lowStockItems.ToListAsync(ct);
    }

    private async Task<List<SupplierBreakdownResponse>> GetSupplierBreakdownAsync(CancellationToken ct)
    {
        var request =
            from p in db.BasicProducts
            join s in db.BasicSuppliers on p.SupplierId equals s.Id
            where p.SupplierId.HasValue
            group p by new
            {
                SupplierId = s.Id,
                SupplierName = s.Person.Name
            }
            into g
            orderby g.Sum(p => p.SalesPrice * (decimal)p.Quantity) descending
            select new SupplierBreakdownResponse(
                g.Key.SupplierId,
                g.Key.SupplierName,
                g.Sum(p => (p.CostPrice ?? 0) * (decimal)p.Quantity),
                g.Sum(p => p.SalesPrice * (decimal)p.Quantity),
                g.Sum(p => p.Quantity)
            );
        
        var supplierBreakdown = await request
                .ToListAsync(ct);
        return supplierBreakdown;
    }
}