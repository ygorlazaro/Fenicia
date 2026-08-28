using Fenicia.Common.Data.Models.Basic;
using Fenicia.Module.Basic.Domains.Customer;
using Fenicia.Module.Basic.Domains.Employee;
using Fenicia.Module.Basic.Domains.Inventory;
using Fenicia.Module.Basic.Domains.Inventory.DTOs;
using Fenicia.Module.Basic.Domains.Supplier;

namespace Fenicia.Module.Basic.Domains.Inventory;

public class InventoryService(
    ProductRepository productRepository,
    StockMovementRepository stockMovementRepository,
    OrderDetailRepository orderDetailRepository,
    CustomerRepository customerRepository,
    EmployeeRepository employeeRepository,
    SupplierRepository supplierRepository)
{
    public async Task<InventoryResponse> GetAsync(GetInventoryQuery query, CancellationToken ct)
    {
        var products = await productRepository.GetAllWithCategoryAsync(query.Page, query.PerPage, ct);

        var totalCostPrice = await productRepository.GetTotalCostPriceAsync(ct);
        var totalSalesPrice = await productRepository.GetTotalSalesPriceAsync(ct);
        var totalQuantity = await productRepository.GetTotalQuantityAsync(ct);

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
        var products = await productRepository.GetByCategoryWithCategoryAsync(query.CategoryId, query.Page, query.PerPage, ct);

        var totalCostPrice = await productRepository.GetTotalCostPriceByCategoryAsync(query.CategoryId, ct);
        var totalSalesPrice = await productRepository.GetTotalSalesPriceByCategoryAsync(query.CategoryId, ct);
        var totalQuantity = await productRepository.GetTotalQuantityByCategoryAsync(query.CategoryId, ct);

        return new InventoryResponse
        {
            Items = products.Select(p => new InventoryDetailResponse(p.Id, p.Name, p.Quantity, p.CostPrice, p.SalesPrice, p.CategoryId, p.Category.Name)).ToList(),
            TotalCostPrice = totalCostPrice,
            TotalSalesPrice = totalSalesPrice,
            TotalQuantity = totalQuantity
        };
    }

    public async Task<InventoryResponse> GetByProductAsync(GetInventoryByProductQuery query, CancellationToken ct)
    {
        var products = await productRepository.GetByIdWithCategoryAsync(query.ProductId, query.Page, query.PerPage, ct);

        var totalCostPrice = await productRepository.GetTotalCostPriceByProductAsync(query.ProductId, ct);
        var totalSalesPrice = await productRepository.GetTotalSalesPriceByProductAsync(query.ProductId, ct);
        var totalQuantity = await productRepository.GetTotalQuantityByProductAsync(query.ProductId, ct);

        return new InventoryResponse
        {
            Items = products.Select(p => new InventoryDetailResponse(p.Id, p.Name, p.Quantity, p.CostPrice, p.SalesPrice, p.CategoryId, p.Category.Name)).ToList(),
            TotalCostPrice = totalCostPrice,
            TotalSalesPrice = totalSalesPrice,
            TotalQuantity = totalQuantity
        };
    }

    public async Task<InventoryDashboardResponse> GetDashboardAsync(GetInventoryDashboardQuery query, CancellationToken ct)
    {
        var lowStockItems = await productRepository.GetLowStockAsync(ct);
        var totalCustomers = await customerRepository.CountAsync(ct);
        var totalEmployees = await employeeRepository.CountAsync(ct);
        var totalCostValue = await productRepository.GetTotalCostValueAsync(ct);
        var totalSalesValue = await productRepository.GetTotalSalesValueAsync(ct);
        var totalQuantity = await productRepository.GetTotalQuantityAsync(ct);
        var profitPotential = totalSalesValue - totalCostValue;
        var categoryBreakdown = await productRepository.GetCategoryBreakdownAsync(ct);
        var supplierBreakdown = await supplierRepository.GetSupplierBreakdownAsync(ct);

        return new InventoryDashboardResponse
        {
            LowStockItems = lowStockItems.Select(p => new InventoryDashboardItemResponse(p.Id, p.Name, p.Quantity, p.CostPrice, p.SalesPrice, p.CategoryId, p.Category.Name)).ToList(),
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
        var stockMovements = await stockMovementRepository.GetByDateRangeAsync(DateTime.UtcNow.AddDays(-query.ZeroMovementDays), ct);
        var orderDetails = await orderDetailRepository.GetByDateRangeAsync(DateTime.UtcNow.AddDays(-query.ZeroMovementDays), ct);

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

    private async Task<(IEnumerable<Guid> activeProductIds, List<ZeroMovementProductResponse> zeroMovementProducts)> GetActiveProductIdsAsync(IEnumerable<StockMovementModel> stockMovements, IEnumerable<OrderDetailModel> orderDetails, CancellationToken ct)
    {
        var movementProductIds = stockMovements.Select(m => m.ProductId).Distinct().ToList();
        var orderProductIds = orderDetails.Select(d => d.ProductId).Distinct().ToList();
        var activeProductIds = movementProductIds.Union(orderProductIds).ToHashSet();

        var candidateProducts = await productRepository.GetZeroMovementCandidatesAsync(activeProductIds, ct);

        var candidateIds = candidateProducts.Select(p => p.Id).ToList();
        var lastMovements = await stockMovementRepository.GetLastMovementsByProductIdsAsync(candidateIds, ct);

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
                    p.Supplier.Person.Name,
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

    private async Task<(List<OverstockProductResponse>, OverstockAlertResponse)> GetOverstockProductsAsync(GetInventoryHealthQuery query, IEnumerable<OrderDetailModel> orderDetails, CancellationToken ct)
    {
        var productSalesRaw = orderDetails.GroupBy(d => d.ProductId).Select(g => new { ProductId = g.Key, TotalSales = g.Sum(d => d.Quantity) }).ToList();

        var productSales = productSalesRaw.ToDictionary(x => x.ProductId, x => x.TotalSales / (query.ZeroMovementDays / 30.0));

        var allProductsWithStock = await productRepository.GetOverstockCandidatesAsync(ct);

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
            Products = overstockProducts.Take(20)
                .ToList()
        };

        return (overstockProducts, overstockAlert);
    }

    private async Task<InventoryHealthSummaryResponse> GetInventoryHealthSummaryAsync(IEnumerable<Guid> activeProductIds, List<OverstockProductResponse> overstockProducts, IEnumerable<ZeroMovementProductResponse> zeroMovementProducts, decimal totalStockValue, CancellationToken ct)
    {
        var totalProducts = await productRepository.CountAsync(p => p.Quantity > 0, ct);
        var totalZeroMovementProducts = zeroMovementProducts.Count();
        var overstockCount = overstockProducts.Count;

        var overstockPercentage = totalProducts > 0 ? (decimal)overstockCount / totalProducts * 100 : 0;
        var zeroMovementPercentage = totalProducts > 0 ? (decimal)totalZeroMovementProducts / totalProducts * 100 : 0;

        var stockedActiveIds = activeProductIds.Where(id => !overstockProducts.Any(op => op.ProductId == id)).ToHashSet();
        var healthyProducts = await productRepository.CountAsync(p => p.Quantity > 0 && stockedActiveIds.Contains(p.Id), ct);

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
        var productsByCategory = await productRepository.GetStockValueByCategoryAsync(ct);

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
}
