#pragma warning disable CS8603,CS8602,CS8604,CS8632
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fenicia.Common;
using Fenicia.Common.Data.Models.Basic;
using Fenicia.Common.DTOs.Basic.Inventory;
using Fenicia.Common.DTOs.Basic.Product;
using Fenicia.Module.Basic.Domains.Customer.Interfaces;
using Fenicia.Module.Basic.Domains.Employee.Interfaces;
using Fenicia.Module.Basic.Domains.Inventory.Interfaces;
using Fenicia.Module.Basic.Domains.OrderDetail.Interfaces;
using Fenicia.Module.Basic.Domains.Product.Interfaces;
using Fenicia.Module.Basic.Domains.StockMovement.Interfaces;
using Fenicia.Module.Basic.Domains.Supplier.Interfaces;

namespace Fenicia.Module.Basic.Domains.Inventory;

/// <summary>
/// Service implementation for managing inventory.
/// </summary>
/// <param name="productService">The product service.</param>
/// <param name="stockMovementService">The stock movement service.</param>
/// <param name="orderDetailService">The order detail service.</param>
/// <param name="customerService">The customer service.</param>
/// <param name="employeeService">The employee service.</param>
/// <param name="supplierService">The supplier service.</param>
public sealed class InventoryService(
    IProductService productService,
    IStockMovementService stockMovementService,
    IOrderDetailService orderDetailService,
    ICustomerService customerService,
    IEmployeeService employeeService,
    ISupplierService supplierService) : IInventoryService
{
    /// <inheritdoc />
    public async Task<InventoryResponse> GetAsync(
        InventoryRequest request,
        CancellationToken cancellationToken = default)
    {
        var products = await productService.GetAllWithCategoryAsync(
            new ProductQuery(request.Page, request.PerPage, request.Query, request.Sort) { Filters = request.Filters },
            cancellationToken);

        var totalCostPrice = await productService.GetTotalCostPriceAsync(cancellationToken);
        var totalSalesPrice = await productService.GetTotalSalesPriceAsync(cancellationToken);
        var totalQuantity = await productService.GetTotalQuantityAsync(cancellationToken);

        var inventoryDetailResponses = products.Data.Select(p => InventoryMapper.MapToDetailResponse(p)).ToList();

        return new InventoryResponse
        {
            Items = inventoryDetailResponses,
            TotalCostPrice = totalCostPrice,
            TotalSalesPrice = totalSalesPrice,
            TotalQuantity = totalQuantity
        };
    }

    /// <inheritdoc />
    public async Task<InventoryResponse> GetByCategoryAsync(
        InventoryByCategoryRequest request,
        CancellationToken cancellationToken = default)
    {
        var products = await productService.GetByCategoryWithCategoryAsync(
            new ProductsByCategoryIdRequest
            (
                request.CategoryId,
                request.Page,
                request.PerPage
            ),
            cancellationToken);

        var totalCostPrice = await productService.GetTotalCostPriceByCategoryAsync(request.CategoryId, cancellationToken);
        var totalSalesPrice =
            await productService.GetTotalSalesPriceByCategoryAsync(request.CategoryId, cancellationToken);
        var totalQuantity = await productService.GetTotalQuantityByCategoryAsync(request.CategoryId, cancellationToken);

        return new InventoryResponse
        {
            Items = [.. products.Data.Select(p => InventoryMapper.MapToDetailResponse(p))],
            TotalCostPrice = totalCostPrice,
            TotalSalesPrice = totalSalesPrice,
            TotalQuantity = totalQuantity
        };
    }

    /// <inheritdoc />
    public async Task<InventoryResponse> GetByProductAsync(
        InventoryByProductRequest request,
        CancellationToken cancellationToken = default)
    {
        var product = await productService.GetByIdWithCategoryAsync(
            request.ProductId,
            cancellationToken);
        var products = product != null
            ? new[] { product }
            : Enumerable.Empty<ProductResponse>();

        var totalCostPrice = await productService.GetTotalCostPriceByProductAsync(request.ProductId, cancellationToken);
        var totalSalesPrice = await productService.GetTotalSalesPriceByProductAsync(request.ProductId, cancellationToken);
        var totalQuantity = await productService.GetTotalQuantityByProductAsync(request.ProductId, cancellationToken);

        return new InventoryResponse
        {
            Items = [.. products.Select(p => InventoryMapper.MapToDetailResponse(p))],
            TotalCostPrice = totalCostPrice,
            TotalSalesPrice = totalSalesPrice,
            TotalQuantity = totalQuantity
        };
    }

    /// <inheritdoc />
    public async Task<InventoryDashboardResponse> GetDashboardAsync(
        InventoryDashboardRequest request,
        CancellationToken cancellationToken = default)
    {
        var endDate = DateTime.UtcNow;
        var startDate = endDate.AddDays(-request.Days);

        var lowStockItems = await productService.GetLowStockAsync(new PaginationQuery(), cancellationToken);
        var orderDetails = await orderDetailService.GetByOrderDateRangeAsync(startDate, endDate, cancellationToken);
        var soldProductIds = orderDetails.Select(d => d.ProductId).Distinct().ToHashSet();
        var filteredLowStock = lowStockItems.Data.AsEnumerable().Where(p => soldProductIds.Contains(p.Id)).Take(10).ToList();

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
            LowStockItems = [.. filteredLowStock.Select(p => InventoryMapper.MapToDetailResponse(p))],
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

    /// <inheritdoc />
    public async Task<InventoryHealthResponse> GetHealthAsync(
        InventoryHealthRequest request,
        CancellationToken cancellationToken = default)
    {
        var stockMovements = await stockMovementService.GetByDateRangeAsync(
            DateTime.UtcNow.AddDays(-request.ZeroMovementDays),
            DateTime.MaxValue,
            cancellationToken);
        var orderDetails = await orderDetailService.GetByOrderDateRangeAsync(
            DateTime.UtcNow.AddDays(-request.ZeroMovementDays),
            DateTime.MaxValue,
            cancellationToken);

        // Map OrderDetailResponse to OrderDetailModel for the method
        var orderDetailModels = orderDetails.Select(od => new OrderDetailModel
        {
            Id = od.Id,
            OrderId = od.OrderId,
            ProductId = od.ProductId,
            Price = od.Price,
            Quantity = od.Quantity,
            DiscountAmount = od.DiscountAmount,
            Subtotal = od.Subtotal
        });

        var result = await GetActiveProductIdsAsync(
            stockMovements,
            orderDetailModels,
            cancellationToken);
        var activeProductIds = result.ActiveProductIds!;
        var zeroMovementProducts = result.ZeroMovementProducts!;
        var tuple = GetStockValueByCategoryFromZeroMovement(zeroMovementProducts);
        var stockValueByCategory = tuple.StockValueByCategories!;
        var totalStockValue = tuple.TotalStockValue;
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

    /// <summary>
    /// Gets stock value by category from zero movement products.
    /// </summary>
    /// <param name="zeroMovementProducts">The zero movement products.</param>
    /// <returns>A tuple containing stock value by category list and total stock value.</returns>
    private static (List<StockValueByCategoryResponse> StockValueByCategories, decimal TotalStockValue)
        GetStockValueByCategoryFromZeroMovement(IEnumerable<ZeroMovementProductResponse> zeroMovementProducts)
    {
        var grouped = zeroMovementProducts
            .GroupBy(p => new { p.CategoryId, p.CategoryName })
            .Select(g =>
            {
                var totalValue = g.Sum(p => p.StockValue);
                return new StockValueByCategoryResponse(g.Key.CategoryId, g.Key.CategoryName, g.Count(), totalValue, 0);
            })
            .OrderByDescending(g => g.TotalStockValue)
            .ToList();

        var totalStockValue = grouped.Sum(g => g.TotalStockValue);

        return (
        [
            .. grouped.Select(s =>
            {
                s.TotalStockValue = totalStockValue > 0 ? s.TotalStockValue / totalStockValue * 100 : 0;
                return s;
            })
        ], totalStockValue);
    }

    /// <summary>
    /// Gets active product IDs from stock movements and order details.
    /// </summary>
    /// <param name="stockMovements">The stock movements.</param>
    /// <param name="orderDetails">The order details.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>A tuple containing active product IDs and zero movement products.</returns>
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
            await productService.GetZeroMovementCandidatesAsync(new PaginationQuery(1, activeProductIds.Count), cancellationToken);

        var candidateIds = candidateProducts.Data.Select(p => p.Id).ToList();
        var lastMovements =
            await stockMovementService.GetLastMovementsByProductIdsAsync(candidateIds, cancellationToken);

        var now = DateTime.UtcNow;
        var ancient = now.AddYears(-100);

        var zeroMovementProducts = candidateProducts.Data
            .Select(p =>
            {
                var lastDate = lastMovements.TryGetValue(p.Id, out var date) ? date : null;
                var daysWithoutMovement = lastDate.HasValue ? (int)(now - lastDate.Value).TotalDays : 999;
                var stockValue = (p.CostPrice ?? 0m) * (decimal)p.Quantity;
                return new ZeroMovementProductResponse(
                    p.Id,
                    p.Name,
                    p.CategoryId,
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

    /// <summary>
    /// Gets the inventory health summary.
    /// </summary>
    /// <param name="activeProductIds">The active product IDs.</param>
    /// <param name="zeroMovementProducts">The zero movement products.</param>
    /// <param name="totalStockValue">The total stock value.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>The inventory health summary response.</returns>
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
}