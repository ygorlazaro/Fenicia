using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Basic;
using Fenicia.Module.Basic.Domains.Inventory.DTOs.Queries;
using Fenicia.Module.Basic.Domains.Inventory.DTOs.Responses;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Domains.Inventory;

public class InventoryService(DefaultContext db)
{
    public async Task<InventoryResponse> GetAsync(GetInventoryQuery query, CancellationToken ct)
    {
        var products = db.BasicProducts.Include(p => p.Category).OrderBy(p => p.Quantity).Skip((query.Page - 1) * query.PerPage).Take(query.PerPage);

        var totalCostPrice = await db.BasicProducts.SumAsync(p => p.CostPrice ?? 0, ct);
        var totalSalesPrice = await db.BasicProducts.SumAsync(p => p.SalesPrice, ct);
        var totalQuantity = await db.BasicProducts.SumAsync(p => p.Quantity, ct);

        var inventoryDetailResponses = products.Select(p => new InventoryDetailResponse(p.Id, p.Name, p.Quantity, p.CostPrice, p.SalesPrice, p.CategoryId, p.Category.Name)).ToList();

        return new InventoryResponse
        {
            Items = inventoryDetailResponses,
            TotalCostPrice = totalCostPrice,
            TotalSalesPrice = totalSalesPrice,
            TotalQuantity = totalQuantity
        };
    }

    public async Task<InventoryResponse> GetByCategoryAsync(GetInventoryByCategoryQuery query, CancellationToken ct)
    {
        var products = db.BasicProducts.Where(p => p.CategoryId == query.CategoryId).Include(p => p.Category).OrderBy(p => p.Quantity).Skip((query.Page - 1) * query.PerPage).Take(query.PerPage);

        var totalCostPrice = await db.BasicProducts.Where(p => p.CategoryId == query.CategoryId).SumAsync(p => p.CostPrice ?? 0, ct);
        var totalSalesPrice = await db.BasicProducts.Where(p => p.CategoryId == query.CategoryId).SumAsync(p => p.SalesPrice, ct);
        var totalQuantity = await db.BasicProducts.Where(p => p.CategoryId == query.CategoryId).SumAsync(p => p.Quantity, ct);

        return new InventoryResponse
        {
            Items = products.Select(p => new InventoryDetailResponse(p.Id,
                    p.Name,
                    p.Quantity,
                    p.CostPrice,
                    p.SalesPrice,
                    p.CategoryId,
                    p.Category.Name))
                .ToList(),
            TotalCostPrice = totalCostPrice,
            TotalSalesPrice = totalSalesPrice,
            TotalQuantity = totalQuantity
        };
    }

    public async Task<InventoryResponse> GetByProductAsync(GetInventoryByProductQuery query, CancellationToken ct)
    {
        var products = db.BasicProducts.Where(p => p.Id == query.ProductId).Include(p => p.Category).OrderBy(p => p.Quantity).Skip((query.Page - 1) * query.PerPage).Take(query.PerPage);

        var totalCostPrice = await db.BasicProducts.Where(p => p.Id == query.ProductId).SumAsync(p => p.CostPrice ?? 0, ct);
        var totalSalesPrice = await db.BasicProducts.Where(p => p.Id == query.ProductId).SumAsync(p => p.SalesPrice, ct);
        var totalQuantity = await db.BasicProducts.Where(p => p.Id == query.ProductId).SumAsync(p => p.Quantity, ct);

        return new InventoryResponse
        {
            Items = products.Select(p => new InventoryDetailResponse(p.Id,
                    p.Name,
                    p.Quantity,
                    p.CostPrice,
                    p.SalesPrice,
                    p.CategoryId,
                    p.Category.Name))
                .ToList(),
            TotalCostPrice = totalCostPrice,
            TotalSalesPrice = totalSalesPrice,
            TotalQuantity = totalQuantity
        };
    }

    public async Task<InventoryDashboardResponse> GetDashboardAsync(GetInventoryDashboardQuery query, CancellationToken ct)
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

    public async Task<InventoryHealthResponse> GetHealthAsync(GetInventoryHealthQuery query, CancellationToken ct)
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

    private async Task<List<CategoryBreakdownResponse>> GetCategoryBreakdownAsync(CancellationToken ct)
    {
        var request = from p in db.BasicProducts group p by new { p.CategoryId, CategoryName = p.Category.Name } into g select new CategoryBreakdownResponse(g.Key.CategoryId, g.Key.CategoryName, g.Sum(p => p.CostPrice.Value * (decimal)p.Quantity), g.Sum(p => p.SalesPrice * (decimal)p.Quantity), g.Sum(p => p.Quantity));

        return await request.ToListAsync(ct);
    }

    private async Task<List<InventoryDashboardItemResponse>> GetInventoryDashboardItemAsync(CancellationToken ct)
    {
        var lowStockItems = db.BasicProducts.OrderBy(p => p.Quantity).Take(5).Select(p => new InventoryDashboardItemResponse(p.Id, p.Name, p.Quantity, p.CostPrice, p.SalesPrice, p.CategoryId, p.Category.Name));

        return await lowStockItems.ToListAsync(ct);
    }

    private async Task<List<SupplierBreakdownResponse>> GetSupplierBreakdownAsync(CancellationToken ct)
    {
        var request = from p in db.BasicProducts join s in db.BasicSuppliers on p.SupplierId equals s.Id where p.SupplierId.HasValue group p by new { SupplierId = s.Id, SupplierName = s.Person.Name } into g orderby g.Sum(p => p.SalesPrice * (decimal)p.Quantity) descending select new SupplierBreakdownResponse(g.Key.SupplierId, g.Key.SupplierName, g.Sum(p => (p.CostPrice ?? 0) * (decimal)p.Quantity), g.Sum(p => p.SalesPrice * (decimal)p.Quantity), g.Sum(p => p.Quantity));

        var supplierBreakdown = await request.ToListAsync(ct);
        return supplierBreakdown;
    }

    private async Task<InventoryHealthSummaryResponse> GetInventoryHealthSummaryAsync(IEnumerable<Guid> activeProductIds, List<OverstockProductResponse> overstockProducts, IEnumerable<ZeroMovementProductResponse> zeroMovementProducts, decimal totalStockValue, CancellationToken ct)
    {
        var totalProducts = await db.BasicProducts.CountAsync(p => p.Quantity > 0, ct);
        var totalZeroMovementProducts = zeroMovementProducts.Count();
        var overstockCount = overstockProducts.Count;

        var overstockPercentage = totalProducts > 0 ? (decimal)overstockCount / totalProducts * 100 : 0;
        var zeroMovementPercentage = totalProducts > 0 ? (decimal)totalZeroMovementProducts / totalProducts * 100 : 0;

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

        var productsByCategory = await (from p in db.BasicProducts
                                        where p.Quantity > 0
                                        select new { p.CategoryId, CategoryName = p.Category.Name, p.Quantity, p.CostPrice })
                                        .ToListAsync(ct);

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

        var movementProductIds = await stockMovements.Select(m => m.ProductId).Distinct().ToListAsync(ct);
        var orderProductIds = await orderDetails.Select(d => d.ProductId).Distinct().ToListAsync(ct);
        var activeProductIds = movementProductIds.Union(orderProductIds).ToHashSet();

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

        var candidateIds = candidateProducts.Select(p => p.Id).ToList();
        var lastMovements = await stockMovements
            .Where(m => candidateIds.Contains(m.ProductId))
            .GroupBy(m => m.ProductId)
            .Select(g => new { ProductId = g.Key, LastDate = g.OrderByDescending(m => m.Date).Select(m => m.Date).FirstOrDefault() })
            .ToDictionaryAsync(k => k.ProductId, v => v.LastDate, ct);

        var now = DateTime.UtcNow;
        var ancient = now.AddYears(-100);

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
