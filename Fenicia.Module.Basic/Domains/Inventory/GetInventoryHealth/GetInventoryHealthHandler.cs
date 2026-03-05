using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Enums.Basic;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Domains.Inventory.GetInventoryHealth;

public class GetInventoryHealthHandler(DefaultContext context)
{
    public async Task<InventoryHealthResponse> Handle(GetInventoryHealthQuery query, CancellationToken ct)
    {
        var products = await context.BasicProducts
            .Include(p => p.Category)
            .Include(p => p.Supplier)
            .ThenInclude(s => s != null ? s.Person : null)
            .ToListAsync(ct);

        var stockMovements = await context.BasicStockMovements
            .Where(m => m.Date >= DateTime.UtcNow.AddDays(-query.ZeroMovementDays))
            .ToListAsync(ct);

        var orderDetails = await context.BasicOrderDetails
            .Include(d => d.OrderModel)
            .Where(d => d.OrderModel.SaleDate >= DateTime.UtcNow.AddDays(-query.ZeroMovementDays))
            .ToListAsync(ct);

        // Calculate average sales per product
        var productSales = orderDetails
            .GroupBy(d => d.ProductId)
            .ToDictionary(
                g => g.Key,
                g => g.Sum(d => d.Quantity) / (query.ZeroMovementDays / 30.0)); // Monthly average

        // 1. Overstock Alert
        var overstockProducts = products
            .Where(p => p.Quantity > 0 && productSales.ContainsKey(p.Id))
            .Select(p =>
            {
                var avgMonthlySales = productSales[p.Id];
                var recommendedQuantity = avgMonthlySales * query.OverstockMultiplier;
                var excessQuantity = Math.Max(0, p.Quantity - recommendedQuantity);
                var excessValue = (decimal)excessQuantity * (p.CostPrice ?? 0);

                return new OverstockProductResponse(
                    p.Id,
                    p.Name,
                    p.Category.Name,
                    p.Quantity,
                    recommendedQuantity,
                    excessValue,
                    p.CostPrice ?? 0);
            })
            .Where(p => p.ExcessValue > 0)
            .OrderByDescending(p => p.ExcessValue)
            .ToList();

        var overstockAlert = new OverstockAlertResponse
        {
            TotalOverstockProducts = overstockProducts.Count,
            TotalOverstockValue = overstockProducts.Sum(p => p.ExcessValue),
            Products = overstockProducts.Take(20).ToList()
        };

        // 2. Zero Movement Products
        var movementProductIds = stockMovements.Select(m => m.ProductId).Distinct().ToHashSet();
        var orderProductIds = orderDetails.Select(d => d.ProductId).Distinct().ToHashSet();
        var activeProductIds = movementProductIds.Union(orderProductIds);

        var zeroMovementProducts = products
            .Where(p => p.Quantity > 0 && !activeProductIds.Contains(p.Id))
            .Select(p =>
            {
                var lastMovement = stockMovements
                    .Where(m => m.ProductId == p.Id)
                    .OrderByDescending(m => m.Date)
                    .FirstOrDefault();

                // var daysWithoutMovement = lastMovement != null
                //     ? (int)(DateTime.UtcNow - lastMovement.Date).TotalDays
                //     : 999;

                return new ZeroMovementProductResponse(
                    p.Id,
                    p.Name,
                    p.Category.Name,
                    p.Supplier?.Person?.Name,
                    p.Quantity,
                    (p.CostPrice ?? 0) * (decimal)p.Quantity,
                    lastMovement?.Date,
                    0);
            })
            .OrderByDescending(p => p.DaysWithoutMovement)
            .ThenByDescending(p => p.StockValue)
            .Take(20)
            .ToList();

        // 3. Stock Value by Category
        var stockValueByCategory = products
            .Where(p => p.Quantity > 0)
            .GroupBy(p => new { p.CategoryId, CategoryName = p.Category.Name })
            .Select(g =>
            {
                var totalValue = g.Sum(p => (p.CostPrice ?? 0) * (decimal)p.Quantity);
                return new StockValueByCategoryResponse(
                    g.Key.CategoryId,
                    g.Key.CategoryName,
                    g.Count(),
                    totalValue,
                    0); // Will calculate percentage below
            })
            .OrderByDescending(c => c.TotalStockValue)
            .ToList();

        var totalStockValue = stockValueByCategory.Sum(c => c.TotalStockValue);
        stockValueByCategory = stockValueByCategory
            .Select(c => new StockValueByCategoryResponse(
                c.CategoryId,
                c.CategoryName,
                c.ProductCount,
                c.TotalStockValue,
                totalStockValue > 0 ? (double)(c.TotalStockValue / totalStockValue * 100) : 0))
            .ToList();

        // 4. Summary
        var healthyProducts = products.Count(p =>
            p.Quantity > 0 &&
            activeProductIds.Contains(p.Id) &&
            !overstockProducts.Any(op => op.ProductId == p.Id));

        var summary = new InventoryHealthSummaryResponse
        {
            TotalProducts = products.Count(p => p.Quantity > 0),
            HealthyProducts = healthyProducts,
            OverstockProducts = overstockProducts.Count,
            ZeroMovementProducts = zeroMovementProducts.Count,
            TotalStockValue = totalStockValue,
            OverstockPercentage = products.Count(p => p.Quantity > 0) > 0
                ? (decimal)overstockProducts.Count / products.Count(p => p.Quantity > 0) * 100
                : 0,
            ZeroMovementPercentage = products.Count(p => p.Quantity > 0) > 0
                ? (decimal)zeroMovementProducts.Count / products.Count(p => p.Quantity > 0) * 100
                : 0
        };

        return new InventoryHealthResponse
        {
            OverstockAlert = overstockAlert,
            ZeroMovementProducts = zeroMovementProducts,
            StockValueByCategory = stockValueByCategory,
            Summary = summary
        };
    }
}
