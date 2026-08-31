using Fenicia.Common.Data.Models.Basic;
using Fenicia.Module.Basic.Domains.Customer;
using Fenicia.Module.Basic.Domains.Employee;
using Fenicia.Module.Basic.Domains.Inventory.DTOs;
using Fenicia.Module.Basic.Domains.OrderDetail;
using Fenicia.Module.Basic.Domains.Product;
using Fenicia.Module.Basic.Domains.StockMovement;
using Fenicia.Module.Basic.Domains.Supplier;

namespace Fenicia.Module.Basic.Domains.Inventory;

public class InventoryService
{
    private readonly ProductService _productService;
    private readonly StockMovementService _stockMovementService;
    private readonly OrderDetailService _orderDetailService;
    private readonly CustomerService _customerService;
    private readonly EmployeeService _employeeService;
    private readonly SupplierService _supplierService;

    public InventoryService()
        : this(null!, null!, null!, null!, null!, null!)
    {
    }

    public InventoryService(
        ProductService productService,
        StockMovementService stockMovementService,
        OrderDetailService orderDetailService,
        CustomerService customerService,
        EmployeeService employeeService,
        SupplierService supplierService)
    {
        _productService = productService;
        _stockMovementService = stockMovementService;
        _orderDetailService = orderDetailService;
        _customerService = customerService;
        _employeeService = employeeService;
        _supplierService = supplierService;
    }

    public virtual async Task<InventoryResponse> GetAsync(GetInventoryQuery query, CancellationToken cancellationToken = default)
    {
        var products = await _productService.GetAllWithCategoryAsync(query.Page, query.PerPage, cancellationToken);

        var totalCostPrice = await _productService.GetTotalCostPriceAsync(cancellationToken);
        var totalSalesPrice = await _productService.GetTotalSalesPriceAsync(cancellationToken);
        var totalQuantity = await _productService.GetTotalQuantityAsync(cancellationToken);

        var inventoryDetailResponses = products.Select(p => p.MapToInventoryDetailResponse()).ToList();

        return new InventoryResponse
        {
            Items = inventoryDetailResponses,
            TotalCostPrice = totalCostPrice,
            TotalSalesPrice = totalSalesPrice,
            TotalQuantity = totalQuantity
        };
    }

    public virtual async Task<InventoryResponse> GetByCategoryAsync(GetInventoryByCategoryQuery query, CancellationToken cancellationToken = default)
    {
        var products = await _productService.GetByCategoryWithCategoryAsync(query.CategoryId, query.Page, query.PerPage, cancellationToken);

        var totalCostPrice = await _productService.GetTotalCostPriceByCategoryAsync(query.CategoryId, cancellationToken);
        var totalSalesPrice = await _productService.GetTotalSalesPriceByCategoryAsync(query.CategoryId, cancellationToken);
        var totalQuantity = await _productService.GetTotalQuantityByCategoryAsync(query.CategoryId, cancellationToken);

        return new InventoryResponse
        {
            Items = [.. products.Select(p => p.MapToInventoryDetailResponse())],
            TotalCostPrice = totalCostPrice,
            TotalSalesPrice = totalSalesPrice,
            TotalQuantity = totalQuantity
        };
    }

    public virtual async Task<InventoryResponse> GetByProductAsync(GetInventoryByProductQuery query, CancellationToken cancellationToken = default)
    {
        var products = await _productService.GetByIdWithCategoryAsync(query.ProductId, query.Page, query.PerPage, cancellationToken);

        var totalCostPrice = await _productService.GetTotalCostPriceByProductAsync(query.ProductId, cancellationToken);
        var totalSalesPrice = await _productService.GetTotalSalesPriceByProductAsync(query.ProductId, cancellationToken);
        var totalQuantity = await _productService.GetTotalQuantityByProductAsync(query.ProductId, cancellationToken);

        return new InventoryResponse
        {
            Items = [.. products.Select(p => p.MapToInventoryDetailResponse())],
            TotalCostPrice = totalCostPrice,
            TotalSalesPrice = totalSalesPrice,
            TotalQuantity = totalQuantity
        };
    }

    public virtual async Task<InventoryDashboardResponse> GetDashboardAsync(GetInventoryDashboardQuery query, CancellationToken cancellationToken = default)
    {
        var lowStockItems = await _productService.GetLowStockAsync(cancellationToken);
        var totalCustomers = await _customerService.GetCountAsync(cancellationToken);
        var totalEmployees = await _employeeService.GetTotalEmployeesAsync(cancellationToken);
        var totalCostValue = await _productService.GetTotalCostValueAsync(cancellationToken);
        var totalSalesValue = await _productService.GetTotalSalesValueAsync(cancellationToken);
        var totalQuantity = await _productService.GetTotalQuantityAsync(cancellationToken);
        var profitPotential = totalSalesValue - totalCostValue;
        var categoryBreakdown = await _productService.GetCategoryBreakdownAsync(cancellationToken);
        var supplierBreakdown = await _supplierService.GetSupplierBreakdownAsync(cancellationToken);

        return new InventoryDashboardResponse
        {
            LowStockItems = [.. lowStockItems.Select(p => p.MapToInventoryDashboardItemResponse())],
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

    public virtual async Task<InventoryHealthResponse> GetHealthAsync(GetInventoryHealthQuery query, CancellationToken cancellationToken = default)
    {
        var stockMovements = await _stockMovementService.GetByDateRangeAsync(DateTime.UtcNow.AddDays(-query.ZeroMovementDays), DateTime.MaxValue, cancellationToken);
        var orderDetails = await _orderDetailService.GetByDateRangeAsync(DateTime.UtcNow.AddDays(-query.ZeroMovementDays), cancellationToken);

        var (overstockProducts, overstockAlert) = await GetOverstockProductsAsync(query, orderDetails, cancellationToken);
        var (activeProductIds, zeroMovementProducts) = await GetActiveProductIdsAsync(stockMovements, orderDetails, cancellationToken);
        var (stockValueByCategory, totalStockValue) = await GetStockValueByCategoryAsync(cancellationToken);
        var summary = await GetInventoryHealthSummaryAsync(activeProductIds, overstockProducts, zeroMovementProducts, totalStockValue, cancellationToken);

        return new InventoryHealthResponse
        {
            OverstockAlert = overstockAlert,
            ZeroMovementProducts = zeroMovementProducts,
            StockValueByCategory = stockValueByCategory,
            Summary = summary
        };
    }

    private async Task<(IEnumerable<Guid> ActiveProductIds, List<ZeroMovementProductResponse> ZeroMovementProducts)> GetActiveProductIdsAsync(IEnumerable<StockMovementModel> stockMovements, IEnumerable<OrderDetailModel> orderDetails, CancellationToken cancellationToken = default)
    {
        var movementProductIds = stockMovements.Select(m => m.ProductId).Distinct().ToList();
        var orderProductIds = orderDetails.Select(d => d.ProductId).Distinct().ToList();
        var activeProductIds = movementProductIds.Union(orderProductIds).ToHashSet();

        var candidateProducts = await _productService.GetZeroMovementCandidatesAsync(activeProductIds, cancellationToken);

        var candidateIds = candidateProducts.Select(p => p.Id).ToList();
        var lastMovements = await _stockMovementService.GetLastMovementsByProductIdsAsync(candidateIds, cancellationToken);

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

    private async Task<(List<OverstockProductResponse> OverstockProductResponses, OverstockAlertResponse OverstockProductResponse)> GetOverstockProductsAsync(GetInventoryHealthQuery query, IEnumerable<OrderDetailModel> orderDetails, CancellationToken cancellationToken = default)
    {
        var productSalesRaw = orderDetails.GroupBy(d => d.ProductId).Select(g => new { ProductId = g.Key, TotalSales = g.Sum(d => d.Quantity) }).ToList();

        var productSales = productSalesRaw.ToDictionary(x => x.ProductId, x => x.TotalSales / (query.ZeroMovementDays / 30.0));

        var allProductsWithStock = await _productService.GetOverstockCandidatesAsync(cancellationToken);

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

    private async Task<InventoryHealthSummaryResponse> GetInventoryHealthSummaryAsync(IEnumerable<Guid> activeProductIds, List<OverstockProductResponse> overstockProducts, List<ZeroMovementProductResponse> zeroMovementProducts, decimal totalStockValue, CancellationToken cancellationToken = default)
    {
        var totalProducts = await _productService.CountAsync(p => p.Quantity > 0, cancellationToken);
        var totalZeroMovementProducts = zeroMovementProducts.Count();
        var overstockCount = overstockProducts.Count;

        var overstockPercentage = totalProducts > 0 ? (decimal)overstockCount / totalProducts * 100 : 0;
        var zeroMovementPercentage = totalProducts > 0 ? (decimal)totalZeroMovementProducts / totalProducts * 100 : 0;

        var stockedActiveIds = activeProductIds.Where(id => !overstockProducts.Any(op => op.ProductId == id)).ToHashSet();
        var healthyProducts = await _productService.CountAsync(p => p.Quantity > 0 && stockedActiveIds.Contains(p.Id), cancellationToken);

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

    private async Task<(List<StockValueByCategoryResponse> StockValueByCategories, decimal TotalStockValue)> GetStockValueByCategoryAsync(CancellationToken cancellationToken = default)
    {
        var productsByCategory = await _productService.GetStockValueByCategoryAsync(cancellationToken);

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
