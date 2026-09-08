using Fenicia.Common.Data;
using Fenicia.Common.Data.Models.Basic;
using Fenicia.Module.Basic.Domains.Customer.Interfaces;
using Fenicia.Module.Basic.Domains.Employee.Interfaces;
using Fenicia.Module.Basic.Domains.Inventory.DTOs;
using Fenicia.Module.Basic.Domains.Inventory.Interfaces;
using Fenicia.Module.Basic.Domains.OrderDetail.Interfaces;
using Fenicia.Module.Basic.Domains.Product.DTOs;
using Fenicia.Module.Basic.Domains.Product.Interfaces;
using Fenicia.Module.Basic.Domains.StockMovement.Interfaces;
using Fenicia.Module.Basic.Domains.Supplier.Interfaces;

namespace Fenicia.Module.Basic.Domains.Inventory;

public sealed class InventoryService(
    IProductService productService,
    IStockMovementService stockMovementService,
    IOrderDetailService orderDetailService,
    ICustomerService customerService,
    IEmployeeService employeeService,
    ISupplierService supplierService) : IInventoryService
{
    public InventoryService()
        : this(null!, null!, null!, null!, null!, null!)
    {
    }

    public async Task<InventoryResponse> GetAsync(
        GetInventoryQuery query,
        CancellationToken cancellationToken = default)
    {
        var products = await productService.GetAllWithCategoryAsync(
            new GetAllProductQuery(query.Page, query.PerPage, query.Query, query.Sort) { Filters = query.Filters },
            cancellationToken);

        var totalCostPrice = await productService.GetTotalCostPriceAsync(cancellationToken);
        var totalSalesPrice = await productService.GetTotalSalesPriceAsync(cancellationToken);
        var totalQuantity = await productService.GetTotalQuantityAsync(cancellationToken);

        var inventoryDetailResponses = products.Select(p => p.MapToInventoryDetailResponse()).ToList();

        return new InventoryResponse
        {
            Items = inventoryDetailResponses,
            TotalCostPrice = totalCostPrice,
            TotalSalesPrice = totalSalesPrice,
            TotalQuantity = totalQuantity
        };
    }

    public async Task<InventoryResponse> GetByCategoryAsync(
        GetInventoryByCategoryQuery query,
        CancellationToken cancellationToken = default)
    {
        var products = await productService.GetByCategoryWithCategoryAsync(
            query.CategoryId,
            query.Page,
            query.PerPage,
            cancellationToken);

        var totalCostPrice = await productService.GetTotalCostPriceByCategoryAsync(query.CategoryId, cancellationToken);
        var totalSalesPrice =
            await productService.GetTotalSalesPriceByCategoryAsync(query.CategoryId, cancellationToken);
        var totalQuantity = await productService.GetTotalQuantityByCategoryAsync(query.CategoryId, cancellationToken);

        return new InventoryResponse
        {
            Items = [.. products.Select(p => p.MapToInventoryDetailResponse())],
            TotalCostPrice = totalCostPrice,
            TotalSalesPrice = totalSalesPrice,
            TotalQuantity = totalQuantity
        };
    }

    public async Task<InventoryResponse> GetByProductAsync(
        GetInventoryByProductQuery query,
        CancellationToken cancellationToken = default)
    {
        var products = await productService.GetByIdWithCategoryAsync(
            query.ProductId,
            query.Page,
            query.PerPage,
            cancellationToken);

        var totalCostPrice = await productService.GetTotalCostPriceByProductAsync(query.ProductId, cancellationToken);
        var totalSalesPrice = await productService.GetTotalSalesPriceByProductAsync(query.ProductId, cancellationToken);
        var totalQuantity = await productService.GetTotalQuantityByProductAsync(query.ProductId, cancellationToken);

        return new InventoryResponse
        {
            Items = [.. products.Select(p => p.MapToInventoryDetailResponse())],
            TotalCostPrice = totalCostPrice,
            TotalSalesPrice = totalSalesPrice,
            TotalQuantity = totalQuantity
        };
    }

    public async Task<InventoryDashboardResponse> GetDashboardAsync(
        GetInventoryDashboardQuery query,
        CancellationToken cancellationToken = default)
    {
        var endDate = DateTime.UtcNow;
        var startDate = endDate.AddDays(-query.Days);

        var lowStockItems = await productService.GetLowStockAsync(cancellationToken);
        var orderDetails = await orderDetailService.GetByOrderDateRangeAsync(startDate, endDate, cancellationToken);
        var soldProductIds = orderDetails.Select(d => d.ProductId).Distinct().ToHashSet();
        var filteredLowStock = lowStockItems.Where(p => soldProductIds.Contains(p.Id)).Take(10).ToList();

        var totalCustomers = await customerService.GetCountAsync(cancellationToken);
        var totalEmployees = await employeeService.GetTotalEmployeesAsync(cancellationToken);
        var totalCostValue = await productService.GetTotalCostValueAsync(cancellationToken);
        var totalSalesValue = await productService.GetTotalSalesValueAsync(cancellationToken);
        var totalQuantity = await productService.GetTotalQuantityAsync(cancellationToken);
        var profitPotential = totalSalesValue - totalCostValue;
        var categoryBreakdown = await productService.GetCategoryBreakdownAsync(cancellationToken);
        var supplierBreakdown = await supplierService.GetSupplierBreakdownAsync(cancellationToken);

        return new InventoryDashboardResponse
        {
            LowStockItems = [.. filteredLowStock.Select(p => p.MapToInventoryDashboardItemResponse())],
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

    public async Task<InventoryHealthResponse> GetHealthAsync(
        GetInventoryHealthQuery query,
        CancellationToken cancellationToken = default)
    {
        var stockMovements = await stockMovementService.GetByDateRangeAsync(
            DateTime.UtcNow.AddDays(-query.ZeroMovementDays),
            DateTime.MaxValue,
            cancellationToken);
        var orderDetails = await orderDetailService.GetByDateRangeAsync(
            DateTime.UtcNow.AddDays(-query.ZeroMovementDays),
            cancellationToken);

        var (activeProductIds, zeroMovementProducts) = await GetActiveProductIdsAsync(
            stockMovements,
            orderDetails,
            cancellationToken);
        var (stockValueByCategory, totalStockValue) = await GetStockValueByCategoryAsync(cancellationToken);
        var summary = await GetInventoryHealthSummaryAsync(
            activeProductIds,
            zeroMovementProducts,
            totalStockValue,
            cancellationToken);

        return new InventoryHealthResponse
        {
            ZeroMovementProducts = zeroMovementProducts,
            StockValueByCategory = stockValueByCategory,
            Summary = summary
        };
    }

    private async Task<(IEnumerable<Guid> ActiveProductIds, List<ZeroMovementProductResponse> ZeroMovementProducts)>
        GetActiveProductIdsAsync(
            IEnumerable<StockMovementModel> stockMovements,
            IEnumerable<OrderDetailModel> orderDetails,
            CancellationToken cancellationToken = default)
    {
        var movementProductIds = stockMovements.Select(m => m.ProductId).Distinct().ToList();
        var orderProductIds = orderDetails.Select(d => d.ProductId).Distinct().ToList();
        var activeProductIds = movementProductIds.Union(orderProductIds).ToHashSet();

        var candidateProducts =
            await productService.GetZeroMovementCandidatesAsync(activeProductIds, cancellationToken);

        var candidateIds = candidateProducts.Select(p => p.Id).ToList();
        var lastMovements =
            await stockMovementService.GetLastMovementsByProductIdsAsync(candidateIds, cancellationToken);

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
                     p.Category!.Id,
                     p.Category!.Name,
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

    private async Task<InventoryHealthSummaryResponse> GetInventoryHealthSummaryAsync(
        IEnumerable<Guid> activeProductIds,
        IEnumerable<ZeroMovementProductResponse> zeroMovementProducts,
        decimal totalStockValue,
        CancellationToken cancellationToken = default)
    {
        var totalProducts = await productService.CountAsync(p => p.Quantity > 0, cancellationToken);
        var totalZeroMovementProducts = zeroMovementProducts.Count();

        var zeroMovementPercentage = totalProducts > 0 ? (decimal)totalZeroMovementProducts / totalProducts * 100 : 0;

        var healthyProducts = await productService.CountAsync(
            p => p.Quantity > 0 && activeProductIds.Contains(p.Id),
            cancellationToken);

        var summary = new InventoryHealthSummaryResponse
        {
            TotalProducts = totalProducts,
            HealthyProducts = healthyProducts,
            ZeroMovementProducts = totalZeroMovementProducts,
            TotalStockValue = totalStockValue,
            ZeroMovementPercentage = zeroMovementPercentage
        };
        return summary;
    }

    private async Task<(List<StockValueByCategoryResponse> StockValueByCategories, decimal TotalStockValue)>
        GetStockValueByCategoryAsync(CancellationToken cancellationToken = default)
    {
        var productsByCategory = await productService.GetStockValueByCategoryAsync(cancellationToken);

        var grouped = productsByCategory
            .GroupBy(p => new { p.CategoryId, p.CategoryName })
            .Select(g =>
            {
                var totalValue = g.Sum(p => (p.CostPrice ?? 0m) * p.Quantity);
                return new StockValueByCategoryResponse(g.Key.CategoryId, g.Key.CategoryName, g.Count(), totalValue, 0);
            })
            .OrderByDescending(g => g.TotalStockValue)
            .ToList();

        var totalStockValue = grouped.Sum(g => g.TotalStockValue);

        return (
        [
            .. grouped.Select(s => s with
            {
                TotalStockValue = totalStockValue > 0 ? s.TotalStockValue / totalStockValue * 100 : 0
            })
        ], totalStockValue);
    }
}