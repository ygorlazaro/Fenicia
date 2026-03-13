using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Basic;
using Fenicia.Module.Basic.Domains.Inventory.Queries;
using Fenicia.Module.Basic.Domains.Inventory.Responses;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Domains.Inventory.Handlers;

public class GetInventoryHealthHandler(DefaultContext db)
{
    public async Task<InventoryHealthResponse> Handle(GetInventoryHealthQuery query, CancellationToken ct)
    {
        var stockMovements = db.BasicStockMovements
            .Where(m => m.Date >= DateTime.UtcNow.AddDays(-query.ZeroMovementDays));

        var orderDetails = db.BasicOrderDetails
            .Where(d => d.Order.SaleDate >= DateTime.UtcNow.AddDays(-query.ZeroMovementDays));

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

    private async Task<InventoryHealthSummaryResponse> GetInventoryHealthSummaryAsync(IEnumerable<Guid> activeProductIds,
        List<OverstockProductResponse> overstockProducts, IEnumerable<ZeroMovementProductResponse> zeroMovementProducts, decimal totalStockValue, CancellationToken ct)
    {
        var healthyProducts = await db.BasicProducts.CountAsync(p =>
            p.Quantity > 0 &&
            activeProductIds.Contains(p.Id) && overstockProducts.All(op => op.ProductId != p.Id), ct);
        var totalZeroMovementProducts = zeroMovementProducts.Count();
        
        var summary = new InventoryHealthSummaryResponse
        {
            TotalProducts = await db.BasicProducts.CountAsync(p => p.Quantity > 0, ct),
            HealthyProducts = healthyProducts,
            OverstockProducts = overstockProducts.Count,
            ZeroMovementProducts = totalZeroMovementProducts,
            TotalStockValue = totalStockValue,
            OverstockPercentage = await db.BasicProducts.CountAsync(p => p.Quantity > 0, ct) > 0
                ? (decimal)overstockProducts.Count / await db.BasicProducts.CountAsync(p => p.Quantity > 0, ct) * 100
                : 0,
            ZeroMovementPercentage = await db.BasicProducts.CountAsync(p => p.Quantity > 0, ct) > 0
                ? (decimal)totalZeroMovementProducts / db.BasicProducts.Count(p => p.Quantity > 0) * 100
                : 0
        };
        return summary;
    }

    private async Task<(List<StockValueByCategoryResponse>, decimal totalStockValue)> GetStockValueByCategoryAsync(CancellationToken ct)
    {
        var request = from p in db.BasicProducts
                      where p.Quantity > 0
                      group p by new
                      {
                          p.CategoryId,
                          CategoryName = p.Category.Name
                      }
                      into g
                      let totalValue = g.Sum(p => (p.CostPrice ?? 0) * (decimal)p.Quantity)
                      orderby totalValue descending 
                      select new StockValueByCategoryResponse(
                          g.Key.CategoryId,
                          g.Key.CategoryName,
                          g.Count(),
                          totalValue,
                          0);

        var stockValueByCategory = await request.ToListAsync(ct);
        var totalStockValue = stockValueByCategory.Sum(c => c.TotalStockValue);
     
        return (stockValueByCategory.Select(s => s with
        {
            TotalStockValue = (decimal)(totalStockValue > 0 ? (double)(s.TotalStockValue) / (double)totalStockValue * 100 : 0)
        }).ToList(), totalStockValue);
    }

    private async Task<(IEnumerable<Guid> activeProductIds, List<ZeroMovementProductResponse> zeroMovementProducts)> GetActiveProductIdsAsync(IQueryable<StockMovementModel> stockMovements, IQueryable<OrderDetailModel> orderDetails,
        CancellationToken ct)
    {
        var movementProductIds = stockMovements.Select(m => m.ProductId).Distinct().ToHashSet();
        var orderProductIds = orderDetails.Select(d => d.ProductId).Distinct().ToHashSet();
        var activeProductIds = movementProductIds.Union(orderProductIds);

        var now = DateTime.UtcNow;

        var request = from p in db.BasicProducts
                      join s in db.BasicSuppliers on p.SupplierId equals s.Id
                      where p.Quantity > 0 && !activeProductIds.Contains(p.Id)
                      let lastMovement = stockMovements
                          .Where(m => m.ProductId == p.Id)
                          .OrderByDescending(m => m.Date)
                          .FirstOrDefault()
                      let daysWithoutMovement = lastMovement != null ? (now - lastMovement.Date).Value.TotalDays : 999
                      select new ZeroMovementProductResponse(
                          p.Id,
                          p.Name,
                          p.Category.Name,
                          s.Person.Name,
                          p.Quantity,
                          (p.CostPrice ?? 0) * (decimal)p.Quantity,
                          lastMovement.Date,
                          0);

        var zeroMovementProducts = await request
            .OrderByDescending(p => p.DaysWithoutMovement)
            .ThenByDescending(p => p.StockValue)
            .Take(20)
            .ToListAsync(ct);
        return (activeProductIds, zeroMovementProducts);
    }

    private async Task<(List<OverstockProductResponse>, OverstockAlertResponse)> GetOverstockProductsAsync(GetInventoryHealthQuery query, IQueryable<OrderDetailModel> orderDetails, CancellationToken ct)
    {
        var productSalesRaw = await orderDetails
            .GroupBy(d => d.ProductId)
            .Select(g => new { ProductId = g.Key, TotalSales = g.Sum(d => d.Quantity) })
            .ToListAsync(ct);

        var productSales = productSalesRaw.ToDictionary(
            x => x.ProductId,
            x => x.TotalSales / (query.ZeroMovementDays / 30.0));


        var allProductsWithStock = await (from p in db.BasicProducts
                                          where p.Quantity > 0
                                          select new { p.Id, p.Name, CategoryName = p.Category.Name, p.Quantity, p.CostPrice }).ToListAsync(ct);

        var overstockProducts = allProductsWithStock
            .Where(p => productSales.ContainsKey(p.Id))
            .Select(p =>
            {
                var avgMonthlySales = productSales[p.Id];
                var recommendedQuantity = avgMonthlySales * query.OverstockMultiplier;
                var excessQuantity = Math.Max(0, p.Quantity - recommendedQuantity);
                var excessValue = (decimal)excessQuantity * (p.CostPrice ?? 0);
                if (excessValue > 0)
                {
                    return new OverstockProductResponse(
                        p.Id,
                        p.Name,
                        p.CategoryName,
                        p.Quantity,
                        recommendedQuantity,
                        excessValue,
                        p.CostPrice ?? 0);
                }
                return null;
            })
            .Where(x => x != null)
            .OrderByDescending(x => x!.ExcessValue)
            .Cast<OverstockProductResponse>()
            .ToList();
        var overstockAlert = new OverstockAlertResponse
        {
            TotalOverstockProducts = overstockProducts.Count,
            TotalOverstockValue = overstockProducts.Sum(p => p.ExcessValue),
            Products = overstockProducts.Take(20).ToList()
        };
        
        return (overstockProducts, overstockAlert);
    }
}
