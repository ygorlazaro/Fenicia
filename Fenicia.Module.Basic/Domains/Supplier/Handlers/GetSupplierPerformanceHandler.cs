using Fenicia.Common.Data.Contexts;
using Fenicia.Module.Basic.Domains.Supplier.Queries;
using Fenicia.Module.Basic.Domains.Supplier.Responses;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Domains.Supplier.Handlers;

/// <summary>
///     Handler responsible for retrieving supplier performance analytics.
///     Provides insights including product counts, stock movements, and cost comparisons.
/// </summary>
public class GetSupplierPerformanceHandler(DefaultContext db)
{
    /// <summary>
    ///     Retrieves supplier performance analytics.
    /// </summary>
    /// <param name="query">The query containing days to analyze and top limit.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Performance analytics including summaries, product counts, cost comparisons, and recent movements.</returns>
    public async Task<SupplierPerformanceResponse> Handle(GetSupplierPerformanceQuery query, CancellationToken ct)
    {
        // Bulletproof split query - always translates
        var productStats = await db.BasicProducts.Where(p => p.SupplierId.HasValue).GroupBy(p => p.SupplierId!.Value).Select(g => new { SupplierId = g.Key, ProductCount = g.Count(), TotalCostValue = g.Sum(p => (p.CostPrice ?? 0m) * (decimal)p.Quantity), TotalSalesValue = g.Sum(p => p.SalesPrice * (decimal)p.Quantity) }).ToListAsync(ct);

        var supplierNames = await db.BasicSuppliers.Include(s => s.Person).Where(s => productStats.Select(ps => ps.SupplierId).Contains(s.Id)).Select(s => new { s.Id, s.Person.Name }).ToDictionaryAsync(s => s.Id, s => s.Name, ct);

        var productsPerSupplier = productStats.Where(ps => supplierNames.ContainsKey(ps.SupplierId)).Select(ps => new SupplierProductCountResponse(ps.SupplierId, supplierNames[ps.SupplierId], ps.ProductCount, ps.TotalCostValue, ps.TotalSalesValue)).OrderByDescending(x => x.TotalStockValue).Take(query.TopLimit).ToList();

        // Recent stock movements
        var recentStockMovementsQuery = db.BasicStockMovements.Include(m => m.Product).Where(m => m.SupplierId.HasValue && m.Date >= DateTime.UtcNow.AddDays(-query.Days)).OrderByDescending(m => m.Date).Take(query.TopLimit).Select(m => new SupplierStockMovementResponse(m.Id, m.ProductId, m.Product.Name, m.Quantity, m.Price ?? 0, m.Date!.Value, m.Type.ToString()));

        var recentStockMovements = await recentStockMovementsQuery.ToListAsync(ct);

        // Products with multiple suppliers for cost comparison (unchanged)
        var productsWithMultipleSuppliers = await GetSupplierCostComparisonAsync(query, ct);

        // Summary from productsPerSupplier data
        var summary = new SupplierSummaryResponse { TotalSuppliers = productsPerSupplier.Count, TotalProducts = productsPerSupplier.Sum(s => s.ProductCount), TotalStockValue = productsPerSupplier.Sum(s => s.TotalStockValue), AverageProductsPerSupplier = productsPerSupplier.Any() ? (decimal)productsPerSupplier.Sum(s => s.ProductCount) / productsPerSupplier.Count : 0 };

        return new SupplierPerformanceResponse { ProductsPerSupplier = productsPerSupplier, CostComparison = productsWithMultipleSuppliers, RecentStockMovements = recentStockMovements, Summary = summary };
    }

    private async Task<List<SupplierCostComparisonResponse>> GetSupplierCostComparisonAsync(GetSupplierPerformanceQuery query, CancellationToken ct)
    {
        var productsWithMultipleSuppliers = await db.BasicProducts.Include(p => p.Supplier).ThenInclude(s => s.Person).Where(p => p.SupplierId.HasValue).GroupBy(p => p.Name).Where(g => g.Count() > 1).Select(g => new SupplierCostComparisonResponse(g.Key, g.Select(p => new ProductSupplierPriceResponse(p.SupplierId!.Value, p.Supplier!.Person.Name, p.CostPrice ?? 0, p.SalesPrice, p.SalesPrice > 0 ? (p.SalesPrice - (p.CostPrice ?? 0)) / p.SalesPrice * 100 : 0)).ToList())).Take(query.TopLimit)
            .ToListAsync(ct);

        return productsWithMultipleSuppliers;
    }
}