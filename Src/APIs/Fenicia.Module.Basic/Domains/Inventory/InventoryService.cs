using Fenicia.Common.Data.Models.Basic;
using Fenicia.Module.Basic.Domains.Customer;
using Fenicia.Module.Basic.Domains.Employee;
using Fenicia.Module.Basic.Domains.Inventory.DTOs;
using Fenicia.Module.Basic.Domains.OrderDetail;
using Fenicia.Module.Basic.Domains.Product;
using Fenicia.Module.Basic.Domains.StockMovement;
using Fenicia.Module.Basic.Domains.Supplier;

namespace Fenicia.Module.Basic.Domains.Inventory;

public class InventoryService(
    ProductService productService,
    StockMovementService stockMovementService,
    OrderDetailService orderDetailService,
    CustomerService customerService,
    EmployeeService employeeService,
    SupplierService supplierService)
{
    public async Task<InventoryResponse> GetAsync(GetInventoryQuery query, CancellationToken ct)
    {
        var products = await productService.GetAllWithCategoryAsync(query.Page, query.PerPage, ct);

        var totalCostPrice = await productService.GetTotalCostPriceAsync(ct);
        var totalSalesPrice = await productService.GetTotalSalesPriceAsync(ct);
        var totalQuantity = await productService.GetTotalQuantityAsync(ct);

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
        var products = await productService.GetByCategoryWithCategoryAsync(query.CategoryId, query.Page, query.PerPage, ct);

        var totalCostPrice = await productService.GetTotalCostPriceByCategoryAsync(query.CategoryId, ct);
        var totalSalesPrice = await productService.GetTotalSalesPriceByCategoryAsync(query.CategoryId, ct);
        var totalQuantity = await productService.GetTotalQuantityByCategoryAsync(query.CategoryId, ct);

        return new InventoryResponse
        {
            Items = [.. products.Select(p => new InventoryDetailResponse(p.Id, p.Name, p.Quantity, p.CostPrice, p.SalesPrice, p.CategoryId, p.Category.Name))],
            TotalCostPrice = totalCostPrice,
            TotalSalesPrice = totalSalesPrice,
            TotalQuantity = totalQuantity
        };
    }

    public async Task<InventoryResponse> GetByProductAsync(GetInventoryByProductQuery query, CancellationToken ct)
    {
        var products = await productService.GetByIdWithCategoryAsync(query.ProductId, query.Page, query.PerPage, ct);

        var totalCostPrice = await productService.GetTotalCostPriceByProductAsync(query.ProductId, ct);
        var totalSalesPrice = await productService.GetTotalSalesPriceByProductAsync(query.ProductId, ct);
        var totalQuantity = await productService.GetTotalQuantityByProductAsync(query.ProductId, ct);

        return new InventoryResponse
        {
            Items = [.. products.Select(p => new InventoryDetailResponse(p.Id, p.Name, p.Quantity, p.CostPrice, p.SalesPrice, p.CategoryId, p.Category.Name))],
            TotalCostPrice = totalCostPrice,
            TotalSalesPrice = totalSalesPrice,
            TotalQuantity = totalQuantity
        };
    }

    public async Task<InventoryDashboardResponse> GetDashboardAsync(GetInventoryDashboardQuery query, CancellationToken ct)
    {
        var lowStockItems = await productService.GetLowStockAsync(ct);
        var totalCustomers = await customerService.GetCountAsync(ct);
        var totalEmployees = await employeeService.GetTotalEmployeesAsync(ct);
        var totalCostValue = await productService.GetTotalCostValueAsync(ct);
        var totalSalesValue = await productService.GetTotalSalesValueAsync(ct);
        var totalQuantity = await productService.GetTotalQuantityAsync(ct);
        var profitPotential = totalSalesValue - totalCostValue;
        var categoryBreakdown = await productService.GetCategoryBreakdownAsync(ct);
        var supplierBreakdown = await supplierService.GetSupplierBreakdownAsync(ct);

        return new InventoryDashboardResponse
        {
            LowStockItems = [.. lowStockItems.Select(p => new InventoryDashboardItemResponse(p.Id, p.Name, p.Quantity, p.CostPrice, p.SalesPrice, p.CategoryId, p.Category.Name))],
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
        var stockMovements = await stockMovementService.GetByDateRangeAsync(DateTime.UtcNow.AddDays(-query.ZeroMovementDays), ct);
        var orderDetails = await orderDetailService.GetByDateRangeAsync(DateTime.UtcNow.AddDays(-query.ZeroMovementDays), ct);

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

    private async Task<(IEnumerable<Guid> ActiveProductIds, List<ZeroMovementProductResponse> ZeroMovementProducts)> GetActiveProductIdsAsync(IEnumerable<StockMovementModel> stockMovements, IEnumerable<OrderDetailModel> orderDetails, CancellationToken ct)
    {
        var movementProductIds = stockMovements.Select(m => m.ProductId).Distinct().ToList();
        var orderProductIds = orderDetails.Select(d => d.ProductId).Distinct().ToList();
        var activeProductIds = movementProductIds.Union(orderProductIds).ToHashSet();

        var candidateProducts = await productService.GetZeroMovementCandidatesAsync(activeProductIds, ct);

        var candidateIds = candidateProducts.Select(p => p.Id).ToList();
        var lastMovements = await stockMovementService.GetLastMovementsByProductIdsAsync(candidateIds, ct);

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
                p.Category.Name,
                p.Supplier?.Person.Name,
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

    private async Task<(List<OverstockProductResponse> OverstockProductResponses, OverstockAlertResponse OverstockProductResponse)> GetOverstockProductsAsync(GetInventoryHealthQuery query, IEnumerable<OrderDetailModel> orderDetails, CancellationToken ct)
    {
        var productSalesRaw = orderDetails.GroupBy(d => d.ProductId).Select(g => new { ProductId = g.Key, TotalSales = g.Sum(d => d.Quantity) }).ToList();

        var productSales = productSalesRaw.ToDictionary(x => x.ProductId, x => x.TotalSales / (query.ZeroMovementDays / 30.0));

        var allProductsWithStock = await productService.GetOverstockCandidatesAsync(ct);

        var overstockProducts = allProductsWithStock.Where(p => productSales.ContainsKey(p.Id)).Select(p =>
    {
        var avgMonthlySales = productSales[p.Id];
        var recommendedQuantity = avgMonthlySales * query.OverstockMultiplier;
        var excessQuantity = Math.Max(0, p.Quantity - recommendedQuantity);
        var excessValue = (decimal)excessQuantity * (p.CostPrice ?? 0);
        return excessValue > 0
            ? new OverstockProductResponse(p.Id, p.Name, p.Category.Name, p.Quantity, recommendedQuantity, excessValue, p.CostPrice ?? 0)
            : null;
    }).Where(x => x != null).OrderByDescending(x => x!.ExcessValue).Cast<OverstockProductResponse>().ToList();
        var overstockAlert = new OverstockAlertResponse
        {
            TotalOverstockProducts = overstockProducts.Count,
            TotalOverstockValue = overstockProducts.Sum(p => p.ExcessValue),
            Products = [.. overstockProducts.Take(20)]
        };

        return (overstockProducts, overstockAlert);
    }

    private async Task<InventoryHealthSummaryResponse> GetInventoryHealthSummaryAsync(IEnumerable<Guid> activeProductIds, List<OverstockProductResponse> overstockProducts, IEnumerable<ZeroMovementProductResponse> zeroMovementProducts, decimal totalStockValue, CancellationToken ct)
    {
        var totalProducts = await productService.CountAsync(p => p.Quantity > 0, ct);
        var totalZeroMovementProducts = zeroMovementProducts.Count();
        var overstockCount = overstockProducts.Count;

        var overstockPercentage = totalProducts > 0 ? (decimal)overstockCount / totalProducts * 100 : 0;
        var zeroMovementPercentage = totalProducts > 0 ? (decimal)totalZeroMovementProducts / totalProducts * 100 : 0;

        var stockedActiveIds = activeProductIds.Where(id => !overstockProducts.Any(op => op.ProductId == id)).ToHashSet();
        var healthyProducts = await productService.CountAsync(p => p.Quantity > 0 && stockedActiveIds.Contains(p.Id), ct);

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

    private async Task<(List<StockValueByCategoryResponse> StockValueByCategories, decimal TotalStockValue)> GetStockValueByCategoryAsync(CancellationToken ct)
    {
        var productsByCategory = await productService.GetStockValueByCategoryAsync(ct);

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

        return ([.. grouped.Select(s => s with { TotalStockValue = totalStockValue > 0 ? (decimal)(s.TotalStockValue / totalStockValue * 100) : 0 })], totalStockValue);
    }
}
