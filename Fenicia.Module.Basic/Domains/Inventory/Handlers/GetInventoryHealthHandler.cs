using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Basic;
using Fenicia.Module.Basic.Domains.Inventory.Queries;
using Fenicia.Module.Basic.Domains.Inventory.Responses;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Domains.Inventory.Handlers;

/// <summary>
///     Handler responsible for generating inventory health analysis.
///     Identifies overstock products, zero-movement products, and provides health metrics.
/// </summary>
public class GetInventoryHealthHandler(DefaultContext db)
{
    /// <summary>
    ///     Generates inventory health analysis with alerts and metrics.
    /// </summary>
    /// <param name="query">The query containing health analysis parameters.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Inventory health response with overstock and zero-movement alerts.</returns>
    public async Task<InventoryHealthResponse> Handle(GetInventoryHealthQuery query, CancellationToken ct)
    {
        var stockMovements = db.BasicStockMovements.Where(m => m.Date >= DateTime.UtcNow.AddDays(-query.ZeroMovementDays));

        var orderDetails = db.BasicOrderDetails.Where(d => d.Order.SaleDate >= DateTime.UtcNow.AddDays(-query.ZeroMovementDays));

        var (overstockProducts, overstockAlert) = await GetOverstockProductsAsync(query, orderDetails, ct);
        var (activeProductIds, zeroMovementProducts) = await GetActiveProductIdsAsync(stockMovements, orderDetails, ct);
        var (stockValueByCategory, totalStockValue) = await GetStockValueByCategoryAsync(ct);
        var summary = await GetInventoryHealthSummaryAsync(activeProductIds, overstockProducts, zeroMovementProducts, totalStockValue, ct);

        return new InventoryHealthResponse
        {
            OverstockAlert = overstockAlert,
            ZeroMovementProducts = zeroMovementProducts,
            StockValueByCategory = stockValueByCategory,
            Summary = summary
        };
    }

    private async Task<InventoryHealthSummaryResponse> GetInventoryHealthSummaryAsync(IEnumerable<Guid> activeProductIds, List<OverstockProductResponse> overstockProducts, IEnumerable<ZeroMovementProductResponse> zeroMovementProducts, decimal totalStockValue, CancellationToken ct)
    {
        var totalProducts = await db.BasicProducts.CountAsync(p => p.Quantity > 0, ct);
        var totalZeroMovementProducts = zeroMovementProducts.Count();
        var overstockCount = overstockProducts.Count;

        // Client-side safe calculations
        var overstockPercentage = totalProducts > 0 ? (decimal)overstockCount / totalProducts * 100 : 0;
        var zeroMovementPercentage = totalProducts > 0 ? (decimal)totalZeroMovementProducts / totalProducts * 100 : 0;

        // Healthy: stocked + active - overstock (approximate, but safe)
        var stockedActiveIds = activeProductIds.Where(id => !overstockProducts.Any(op => op.ProductId == id)).ToHashSet();
        var healthyProducts = await db.BasicProducts.CountAsync(p => p.Quantity > 0 && stockedActiveIds.Contains(p.Id), ct);

        var summary = new InventoryHealthSummaryResponse
        {
            TotalProducts = totalProducts,
            HealthyProducts = healthyProducts,
            OverstockProducts = overstockCount,
            ZeroMovementProducts = totalZeroMovementProducts,
            TotalStockValue = totalStockValue,
            OverstockPercentage = overstockPercentage,
            ZeroMovementPercentage = zeroMovementPercentage
        };
        return summary;
    }

    private async Task<(List<StockValueByCategoryResponse>, decimal totalStockValue)> GetStockValueByCategoryAsync(CancellationToken ct)
    {
        // Fetch products with category info
        var productsByCategory = await (from p in db.BasicProducts
                                        where p.Quantity > 0
                                        select new { p.CategoryId, CategoryName = p.Category.Name, p.Quantity, p.CostPrice })
                                        .ToListAsync(ct);

        // Client-side group, aggregate, order
        var grouped = productsByCategory
            .GroupBy(p => new { p.CategoryId, p.CategoryName })
            .Select(g =>
            {
                var totalValue = g.Sum(p => (p.CostPrice ?? 0m) * (decimal)p.Quantity);
                return new StockValueByCategoryResponse(g.Key.CategoryId, g.Key.CategoryName, g.Count(), totalValue, 0);
            })
            .OrderByDescending(g => g.TotalStockValue)
            .ToList();

        var totalStockValue = grouped.Sum(g => g.TotalStockValue);

        return (grouped.Select(s => s with { TotalStockValue = totalStockValue > 0 ? (decimal)(s.TotalStockValue / totalStockValue * 100) : 0 }).ToList(), totalStockValue);
    }

    private async Task<(IEnumerable<Guid> activeProductIds, List<ZeroMovementProductResponse> zeroMovementProducts)> GetActiveProductIdsAsync(IQueryable<StockMovementModel> stockMovements, IQueryable<OrderDetailModel> orderDetails, CancellationToken ct)
    {
        // Get active product IDs from movements and orders
        var movementProductIds = await stockMovements.Select(m => m.ProductId).Distinct().ToListAsync(ct);
        var orderProductIds = await orderDetails.Select(d => d.ProductId).Distinct().ToListAsync(ct);
        var activeProductIds = movementProductIds.Union(orderProductIds).ToHashSet();

        // Get candidate products: stock > 0, not active
        var candidateProducts = await (from p in db.BasicProducts
                                       where p.Quantity > 0 && !activeProductIds.Contains(p.Id)
                                       select new
                                       {
                                           p.Id,
                                           p.Name,
                                           CategoryName = p.Category.Name,
                                           SupplierName = p.Supplier.Person.Name,
                                           p.Quantity,
                                           p.CostPrice,
                                           p.SupplierId
                                       }).ToListAsync(ct);

        // Pre-fetch last movement dates for candidates only (efficient dict)
        var candidateIds = candidateProducts.Select(p => p.Id).ToList();
        var lastMovements = await stockMovements
            .Where(m => candidateIds.Contains(m.ProductId))
            .GroupBy(m => m.ProductId)
            .Select(g => new { ProductId = g.Key, LastDate = g.OrderByDescending(m => m.Date).Select(m => m.Date).FirstOrDefault() })
            .ToDictionaryAsync(k => k.ProductId, v => v.LastDate, ct);

        var now = DateTime.UtcNow;
        var ancient = now.AddYears(-100); // fallback for no movement

        // Project to responses, sort, take top 20
        var zeroMovementProducts = candidateProducts
            .Select(p =>
            {
                var lastDate = lastMovements.TryGetValue(p.Id, out var date) ? date : null;
                var daysWithoutMovement = lastDate.HasValue ? (int)(now - lastDate.Value).TotalDays : 999;
                var stockValue = (p.CostPrice ?? 0m) * (decimal)p.Quantity;
                return new ZeroMovementProductResponse(
                    p.Id,
                    p.Name,
                    p.CategoryName,
                    p.SupplierName,
                    p.Quantity,
                    stockValue,
                    lastDate ?? ancient,
                    daysWithoutMovement);
            })
            .OrderByDescending(p => p.DaysWithoutMovement)
            .ThenByDescending(p => p.StockValue)
            .Take(20)
            .ToList();

        return (activeProductIds, zeroMovementProducts);
    }

    private async Task<(List<OverstockProductResponse>, OverstockAlertResponse)> GetOverstockProductsAsync(GetInventoryHealthQuery query, IQueryable<OrderDetailModel> orderDetails, CancellationToken ct)
    {
        var productSalesRaw = await orderDetails.GroupBy(d => d.ProductId).Select(g => new { ProductId = g.Key, TotalSales = g.Sum(d => d.Quantity) }).ToListAsync(ct);

        var productSales = productSalesRaw.ToDictionary(x => x.ProductId, x => x.TotalSales / (query.ZeroMovementDays / 30.0));


        var allProductsWithStock = await (from p in db.BasicProducts
                                          where p.Quantity > 0
                                          select new
                                          {
                                              p.Id,
                                              p.Name,
                                              CategoryName = p.Category.Name,
                                              p.Quantity,
                                              p.CostPrice
                                          }).ToListAsync(ct);

        var overstockProducts = allProductsWithStock.Where(p => productSales.ContainsKey(p.Id)).Select(p =>
        {
            var avgMonthlySales = productSales[p.Id];
            var recommendedQuantity = avgMonthlySales * query.OverstockMultiplier;
            var excessQuantity = Math.Max(0, p.Quantity - recommendedQuantity);
            var excessValue = (decimal)excessQuantity * (p.CostPrice ?? 0);
            return excessValue > 0
                ? new OverstockProductResponse(p.Id, p.Name, p.CategoryName, p.Quantity, recommendedQuantity, excessValue, p.CostPrice ?? 0)
                : null;
        }).Where(x => x != null).OrderByDescending(x => x!.ExcessValue).Cast<OverstockProductResponse>().ToList();
        var overstockAlert = new OverstockAlertResponse
        {
            TotalOverstockProducts = overstockProducts.Count,
            TotalOverstockValue = overstockProducts.Sum(p => p.ExcessValue),
            Products = overstockProducts.Take(20)
                .ToList()
        };

        return (overstockProducts, overstockAlert);
    }
}
