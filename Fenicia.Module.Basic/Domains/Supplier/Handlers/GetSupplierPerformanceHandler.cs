using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Basic;
using Fenicia.Module.Basic.Domains.Supplier.Queries;
using Fenicia.Module.Basic.Domains.Supplier.Responses;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;

namespace Fenicia.Module.Basic.Domains.Supplier.Handlers;

public class GetSupplierPerformanceHandler(DefaultContext db)
{
    public async Task<SupplierPerformanceResponse> Handle(GetSupplierPerformanceQuery query, CancellationToken ct)
    {
        var suppliers = db.BasicSuppliers
            .Include(s => s.Person)
            .Include(s => s.Products)
            .ThenInclude(p => p.Category);

        var stockMovements = db.BasicStockMovements
            .Include(m => m.Product)
            .Where(m => m.SupplierId.HasValue && m.Date >= DateTime.UtcNow.AddDays(-query.Days));

        var productsPerSupplier = await GetSupplierProductCountAsync(suppliers, ct);
        var productsWithMultipleSuppliers = await GetSupplierCostComparisonAsync(query, ct);
        var recentStockMovements = await GetSupplierStockMovementAsync(query, stockMovements, ct);
        var summary = await GetSupplierSummaryAsync(suppliers, productsPerSupplier, ct);

        return new SupplierPerformanceResponse
        {
            ProductsPerSupplier = productsPerSupplier,
            CostComparison = productsWithMultipleSuppliers,
            RecentStockMovements = recentStockMovements,
            Summary = summary
        };
    }

    private async Task<SupplierSummaryResponse> GetSupplierSummaryAsync(
        IIncludableQueryable<SupplierModel, ProductCategoryModel> suppliers,
        List<SupplierProductCountResponse> productsPerSupplier,
        CancellationToken ct)
    {
        var totalSuppliers = await suppliers.CountAsync(ct);
        
        var summary = new SupplierSummaryResponse
        {
            TotalSuppliers = totalSuppliers,
            TotalProducts = await suppliers.SumAsync(s => s.Products.Count, ct),
            TotalStockValue = productsPerSupplier.Sum(s => s.TotalStockValue),
            AverageProductsPerSupplier = totalSuppliers > 0 
                ? (decimal)await suppliers.SumAsync(s => s.Products.Count, ct) / totalSuppliers 
                : 0
        };
        return summary;
    }

    private async Task<List<SupplierStockMovementResponse>> GetSupplierStockMovementAsync(
        GetSupplierPerformanceQuery query,
        IQueryable<StockMovementModel> stockMovements,
        CancellationToken ct)
    {
        var recentStockMovements = await stockMovements
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
            .ToListAsync(ct);
        return recentStockMovements;
    }

    private async Task<List<SupplierCostComparisonResponse>> GetSupplierCostComparisonAsync(
        GetSupplierPerformanceQuery query,
        CancellationToken ct)
    {
        var productsWithMultipleSuppliers = await db.BasicProducts
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
            .ToListAsync(ct);
        
        return productsWithMultipleSuppliers;
    }

    private async Task<List<SupplierProductCountResponse>> GetSupplierProductCountAsync(
        IIncludableQueryable<SupplierModel, ProductCategoryModel> suppliers,
        CancellationToken ct)
    {
        var productsPerSupplier = await suppliers
            .Select(s => new SupplierProductCountResponse(
                s.Id,
                s.Person.Name,
                s.Products.Count,
                s.Products.Sum(p => (p.CostPrice ?? 0) * (decimal)p.Quantity),
                s.Products.Sum(p => p.SalesPrice * (decimal)p.Quantity)))
            .OrderByDescending(s => s.TotalStockValue)
            .ToListAsync(ct);
        
        return productsPerSupplier;
    }
}
