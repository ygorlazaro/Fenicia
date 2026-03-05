using Fenicia.Common.Data.Contexts;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Domains.Supplier.GetSupplierPerformance;

public class GetSupplierPerformanceHandler(DefaultContext context)
{
    public async Task<SupplierPerformanceResponse> Handle(GetSupplierPerformanceQuery query, CancellationToken ct)
    {
        var suppliers = await context.BasicSuppliers
            .Include(s => s.Person)
            .Include(s => s.Products)
            .ThenInclude(p => p.Category)
            .ToListAsync(ct);

        var stockMovements = await context.BasicStockMovements
            .Include(m => m.Product)
            .Where(m => m.SupplierId.HasValue && m.Date >= DateTime.UtcNow.AddDays(-query.Days))
            .ToListAsync(ct);

        // 1. Products per Supplier
        var productsPerSupplier = suppliers
            .Select(s => new SupplierProductCountResponse(
                s.Id,
                s.Person.Name,
                s.Products.Count,
                s.Products.Sum(p => (p.CostPrice ?? 0) * (decimal)p.Quantity),
                s.Products.Sum(p => p.SalesPrice * (decimal)p.Quantity)))
            .OrderByDescending(s => s.TotalStockValue)
            .ToList();

        // 2. Cost Comparison - Products supplied by multiple suppliers
        var productsWithMultipleSuppliers = context.BasicProducts
            .Include(p => p.Supplier)
            .ThenInclude(s => s != null ? s.Person : null)
            .Where(p => p.SupplierId.HasValue)
            .GroupBy(p => p.Name)
            .Where(g => g.Count() > 1)
            .Select(g => new SupplierCostComparisonResponse(
                g.Key,
                g.Select(p => new ProductSupplierPriceResponse(
                    p.SupplierId!.Value,
                    p.Supplier!.Person.Name,
                    p.CostPrice ?? 0,
                    p.SalesPrice,
                    p.SalesPrice > 0 ? ((p.SalesPrice - (p.CostPrice ?? 0)) / p.SalesPrice) * 100 : 0
                )).ToList()
            ))
            .Take(query.TopLimit)
            .ToList();

        // 3. Recent Stock Movements from Suppliers
        var recentStockMovements = stockMovements
            .Where(m => m.SupplierId.HasValue)
            .Select(m => new SupplierStockMovementResponse(
                m.Id,
                m.ProductId,
                m.Product.Name,
                m.Quantity,
                m.Price ?? 0,
                m.Date!.Value,
                m.Type.ToString()
            ))
            .OrderByDescending(m => m.Date)
            .Take(query.TopLimit)
            .ToList();

        // 4. Summary
        var summary = new SupplierSummaryResponse
        {
            TotalSuppliers = suppliers.Count,
            TotalProducts = suppliers.Sum(s => s.Products.Count),
            TotalStockValue = productsPerSupplier.Sum(s => s.TotalStockValue),
            AverageProductsPerSupplier = suppliers.Count > 0 
                ? (decimal)suppliers.Sum(s => s.Products.Count) / suppliers.Count 
                : 0
        };

        return new SupplierPerformanceResponse
        {
            ProductsPerSupplier = productsPerSupplier,
            CostComparison = productsWithMultipleSuppliers,
            RecentStockMovements = recentStockMovements,
            Summary = summary
        };
    }
}
