using System.Linq.Expressions;
using Fenicia.Common;
using Fenicia.Common.Data;
using Fenicia.Common.Data.Models.Basic;
using Fenicia.Common.DTOs.Basic.DataSource;
using Fenicia.Common.DTOs.Basic.Inventory;
using Fenicia.Common.DTOs.Basic.Order;
using Fenicia.Common.DTOs.Basic.Product;
using Fenicia.Module.Basic.Domains.OrderDetail.Interfaces;
using Fenicia.Module.Basic.Domains.Product.Interfaces;
using Fenicia.Module.Basic.Domains.StockMovement.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Domains.Product;

/// <summary>
/// Service implementation for managing products.
/// </summary>
/// <param name="repository">The product repository.</param>
/// <param name="orderDetailService">The order detail service.</param>
/// <param name="stockMovementService">The stock movement service.</param>
public sealed class ProductService(
    IProductRepository repository,
    IOrderDetailService orderDetailService,
    IStockMovementService stockMovementService) : IProductService
{
    /// <inheritdoc />
    public async Task<Pagination<List<ProductResponse>>> GetAllAsync(
        ProductQuery query,
        CancellationToken cancellationToken = default)
    {
        var baseQuery = repository.Query()
            .Include(p => p.Category)
            .Include(p => p.Supplier).ThenInclude(s => s != null ? s.Person : null);

        var filteredQuery = baseQuery.ApplySearch(query.Query, "Name", "SKU", "Category.Name", "Supplier.Person.Name").ApplyFilters(query.Filters).ApplySort(query.Sort);

        var total = await filteredQuery.CountAsync(cancellationToken);

        var products = await filteredQuery
            .Skip((query.Page - 1) * query.PerPage)
            .Take(query.PerPage)
            .ToListAsync(cancellationToken);

        var response = products.Select(ProductMapper.MapToResponse).ToList();

        return new Pagination<List<ProductResponse>>(response, total, query);
    }

    /// <inheritdoc />
    public async Task<List<ProductDataSourceResponse>> GetAllForDataSourceAsync(
        CancellationToken cancellationToken = default)
    {
        var products = await repository.Query()
            .OrderBy(p => p.Name)
            .Select(p => new ProductDataSourceResponse(p.Id, p.Name))
            .ToListAsync(cancellationToken);

        return products;
    }

    /// <inheritdoc />
    public async Task<List<DashboardProductDataSourceResponse>>
        GetAllDashboardForDataSourceAsync(CancellationToken cancellationToken = default)
    {
        var products = await repository.Query()
            .Include(p => p.Category)
            .OrderBy(p => p.Name)
            .Select(p => new DashboardProductDataSourceResponse(
                p.Id,
                p.Name,
                p.SKU,
                p.Barcode,
                p.CostPrice,
                p.SalesPrice,
                p.Quantity,
                p.UnitOfMeasure,
                p.CategoryId,
                p.Category != null ? p.Category.Name : string.Empty,
                p.IsActive))
            .ToListAsync(cancellationToken);

        return products;
    }

    /// <inheritdoc />
    public async Task<ProductResponse?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var product = await repository.GetByIdWithDetailsAsync(id, cancellationToken);

        return product is null ? null : ProductMapper.MapToResponse(product);
    }

    /// <inheritdoc />
    public async Task<Pagination<List<ProductResponse>>> GetByCategoryIdAsync(
        ProductsByCategoryIdRequest request,
        CancellationToken cancellationToken = default)
    {
        var baseQuery = repository.Query()
            .Where(p => p.CategoryId == request.CategoryId)
            .Include(p => p.Category);

        var filteredQuery = baseQuery.ApplySearch(request.Query, "Name").ApplyFilters(request.Filters).ApplySort(request.Sort);

        var total = await filteredQuery.CountAsync(cancellationToken);

        var products = await filteredQuery
            .Skip((request.Page - 1) * request.PerPage)
            .Take(request.PerPage)
            .ToListAsync(cancellationToken);

        var response = products.Select(ProductMapper.MapToResponse).ToList();

        return new Pagination<List<ProductResponse>>(response, total, request);
    }

    /// <inheritdoc />
    public async Task<ProductResponse> AddAsync(
        ProductRequest command,
        Guid companyId,
        CancellationToken cancellationToken = default)
    {
        var product = new ProductModel
        {
            Id = command.Id,
            Name = command.Name,
            SKU = command.SKU,
            Barcode = command.Barcode,
            Description = command.Description,
            CostPrice = command.CostPrice,
            SalesPrice = command.SalesPrice,
            Quantity = command.Quantity,
            MinStockLevel = command.MinStockLevel,
            MaxStockLevel = command.MaxStockLevel,
            ImageUrl = command.ImageUrl,
            Weight = command.Weight,
            Dimensions = command.Dimensions,
            UnitOfMeasure = command.UnitOfMeasure,
            CategoryId = command.CategoryId,
            SupplierId = command.SupplierId,
            IsActive = true,
            CompanyId = companyId
        };

        await repository.InsertAsync(product, cancellationToken);

        var insertedProduct = await repository.GetByIdWithDetailsAsync(product.Id, cancellationToken);

        return ProductMapper.MapToResponse(insertedProduct!);
    }

    /// <inheritdoc />
    public async Task<ProductResponse?> UpdateAsync(
        Guid id,
        ProductRequest command,
        Guid companyId,
        CancellationToken cancellationToken = default)
    {
        var product = await repository.GetByIdAsync(id, cancellationToken);

        if (product is null)
        {
            return null;
        }

        product.Name = command.Name;
        product.SKU = command.SKU;
        product.Barcode = command.Barcode;
        product.Description = command.Description;
        product.CostPrice = command.CostPrice;
        product.SalesPrice = command.SalesPrice;
        product.Quantity = command.Quantity;
        product.MinStockLevel = command.MinStockLevel;
        product.MaxStockLevel = command.MaxStockLevel;
        product.ImageUrl = command.ImageUrl;
        product.Weight = command.Weight;
        product.Dimensions = command.Dimensions;
        product.UnitOfMeasure = command.UnitOfMeasure;
        product.CategoryId = command.CategoryId;
        product.SupplierId = command.SupplierId;
        product.CompanyId = companyId;

        await repository.UpdateAsync(product.Id, product, cancellationToken);

        var updatedProduct = await repository.GetByIdWithDetailsAsync(product.Id, cancellationToken);

        return ProductMapper.MapToResponse(updatedProduct!);
    }

    /// <inheritdoc />
    public async Task DeleteAsync(
        Guid id,
        Guid companyId,
        CancellationToken cancellationToken = default)
    {
        await repository.DeleteAsync(id, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<ProductPerformanceResponse> GetPerformanceAsync(
        ProductPerformanceRequest request,
        CancellationToken cancellationToken = default)
    {
        var startDate = DateTime.UtcNow.AddDays(-request.Days);
        var endDate = DateTime.UtcNow;

        var products = await repository.GetAllWithDetailsAsync(new PaginationQuery(1, int.MaxValue), cancellationToken);
        var productList = products.ToList();

        var orderDetails = await orderDetailService.GetByOrderDateRangeAsync(startDate, endDate, cancellationToken);
        var orderDetailList = orderDetails.ToList();

        var stockMovements = await stockMovementService.GetByDateRangeAsync(startDate, endDate, cancellationToken);
        var stockMovementList = stockMovements.ToList();

        var bestSellingProducts = await GetBestSellingProductAsync(request, orderDetailList, cancellationToken);
        var worstSellingProducts = GetWorstSellingProduct(request, orderDetailList, productList);
        var profitMargins = GetProfitMarginsList(productList);
        var neverSoldProducts = GetNeverSoldProduct(request, orderDetailList, productList, stockMovementList);

        return new ProductPerformanceResponse
        {
            BestSellingProducts = bestSellingProducts,
            WorstSellingProducts = worstSellingProducts,
            ProfitMargins = profitMargins,
            NeverSoldProducts = neverSoldProducts
        };
    }

    /// <inheritdoc />
    public Task<int> GetCountAsync(CancellationToken cancellationToken = default)
    {
        return repository.CountAsync(cancellationToken);
    }

    /// <inheritdoc />
    public Task<int> GetTotalProductsAsync(CancellationToken cancellationToken = default)
    {
        return repository.CountAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<List<ProductResponse>> GetAllWithSupplierAsync(CancellationToken cancellationToken = default)
    {
        var products = await repository.GetAllWithSupplierAsync(cancellationToken);
        return products.Select(ProductMapper.MapToResponse).ToList();
    }

    /// <inheritdoc />
    public async Task<List<ProductResponse>> GetAllForStatsAsync(CancellationToken cancellationToken = default)
    {
        var products = await repository.GetAllForStatsAsync(cancellationToken);
        return products.Select(ProductMapper.MapToResponse).ToList();
    }

    /// <inheritdoc />
    public async Task<List<ProductResponse>> GetAllWithCategoryAsync(
        ProductQuery query,
        CancellationToken cancellationToken = default)
    {
        var products = await repository.GetAllWithCategoryAsync(query, cancellationToken);
        return products.Select(ProductMapper.MapToResponse).ToList();
    }

    /// <inheritdoc />
    public Task<decimal> GetTotalCostPriceAsync(CancellationToken cancellationToken = default)
    {
        return repository.GetTotalCostPriceAsync(cancellationToken);
    }

    /// <inheritdoc />
    public Task<decimal> GetTotalSalesPriceAsync(CancellationToken cancellationToken = default)
    {
        return repository.GetTotalSalesPriceAsync(cancellationToken);
    }

    /// <inheritdoc />
    public Task<int> GetTotalQuantityAsync(CancellationToken cancellationToken = default)
    {
        return repository.GetTotalQuantityAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<List<ProductResponse>> GetByCategoryWithCategoryAsync(
        Guid categoryId,
        PaginationQuery query,
        CancellationToken cancellationToken = default)
    {
        var products = await repository.GetByCategoryWithCategoryAsync(categoryId, query, cancellationToken);
        return products.Select(ProductMapper.MapToResponse).ToList();
    }

    /// <inheritdoc />
    public Task<decimal> GetTotalCostPriceByCategoryAsync(
        Guid categoryId,
        CancellationToken cancellationToken = default)
    {
        return repository.GetTotalCostPriceByCategoryAsync(categoryId, cancellationToken);
    }

    /// <inheritdoc />
    public Task<decimal> GetTotalSalesPriceByCategoryAsync(
        Guid categoryId,
        CancellationToken cancellationToken = default)
    {
        return repository.GetTotalSalesPriceByCategoryAsync(categoryId, cancellationToken);
    }

    /// <inheritdoc />
    public Task<int> GetTotalQuantityByCategoryAsync(
        Guid categoryId,
        CancellationToken cancellationToken = default)
    {
        return repository.GetTotalQuantityByCategoryAsync(categoryId, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<List<ProductResponse>> GetByIdWithCategoryAsync(
        Guid productId,
        PaginationQuery query,
        CancellationToken cancellationToken = default)
    {
        var products = await repository.GetByIdWithCategoryAsync(productId, query, cancellationToken);
        return products.Select(ProductMapper.MapToResponse).ToList();
    }

    /// <inheritdoc />
    public Task<decimal> GetTotalCostPriceByProductAsync(
        Guid productId,
        CancellationToken cancellationToken = default)
    {
        return repository.GetTotalCostPriceByProductAsync(productId, cancellationToken);
    }

    /// <inheritdoc />
    public Task<decimal> GetTotalSalesPriceByProductAsync(
        Guid productId,
        CancellationToken cancellationToken = default)
    {
        return repository.GetTotalSalesPriceByProductAsync(productId, cancellationToken);
    }

    /// <inheritdoc />
    public Task<int> GetTotalQuantityByProductAsync(
        Guid productId,
        CancellationToken cancellationToken = default)
    {
        return repository.GetTotalQuantityByProductAsync(productId, cancellationToken);
    }

    /// <inheritdoc />
    public Task<List<ProductModel>> GetLowStockAsync(CancellationToken cancellationToken = default)
    {
        return repository.GetLowStockAsync(cancellationToken);
    }

    /// <inheritdoc />
    public Task<decimal> GetTotalCostValueAsync(CancellationToken cancellationToken = default)
    {
        return repository.GetTotalCostValueAsync(cancellationToken);
    }

    /// <inheritdoc />
    public Task<decimal> GetTotalSalesValueAsync(CancellationToken cancellationToken = default)
    {
        return repository.GetTotalSalesValueAsync(cancellationToken);
    }

    /// <inheritdoc />
    public Task<List<ProductModel>> GetZeroMovementCandidatesAsync(
        IEnumerable<Guid> activeProductIds,
        CancellationToken cancellationToken = default)
    {
        return repository.GetZeroMovementCandidatesAsync(activeProductIds, cancellationToken);
    }

    /// <inheritdoc />
    public Task<int> CountAsync(
        Expression<Func<ProductModel, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        return repository.CountAsync(predicate, cancellationToken);
    }

    /// <inheritdoc />
    public Task<List<(Guid CategoryId, string CategoryName, int Quantity, decimal? CostPrice)>>
        GetStockValueByCategoryAsync(CancellationToken cancellationToken = default)
    {
        return repository.GetStockValueByCategoryAsync(cancellationToken);
    }

    /// <inheritdoc />
    public Task<List<CategoryBreakdownResponse>> GetCategoryBreakdownAsync(
        CancellationToken cancellationToken = default)
    {
        return repository.GetCategoryBreakdownAsync(cancellationToken);
    }

    /// <inheritdoc />
    public Task<ProductModel?> GetByIdModelAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return repository.GetByIdAsync(id, cancellationToken);
    }

    /// <inheritdoc />
    public Task<ProductModel?> UpdateAsync(Guid id, ProductModel product, CancellationToken cancellationToken = default)
    {
        return repository.UpdateAsync(id, product, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<List<ProductResponse>> GetAllWithDetailsAsync(
        PaginationQuery query,
        CancellationToken cancellationToken = default)
    {
        var products = await repository.GetAllWithDetailsAsync(query, cancellationToken);
        return products.Select(ProductMapper.MapToResponse).ToList();
    }

    private static string ClassifyMargin(double margin)
    {
        return margin switch
        {
            >= 50 => "Excellent",
            >= 30 => "Good",
            >= 15 => "Average",
            >= 5 => "Low",
            _ => "Very Low"
        };
    }

    private static List<WorstSellingProductResponse> GetWorstSellingProduct(
        ProductPerformanceRequest request,
        IEnumerable<OrderDetailResponse> orderDetails,
        IEnumerable<ProductModel> products)
    {
        var orderDetailList = orderDetails.ToList();
        var productList = products.ToList();

        var salesStats = orderDetailList.GroupBy(d => d.ProductId).Select(g => new
        {
            ProductId = g.Key,
            QuantitySold = g.Sum(d => d.Quantity),
            Revenue = g.Sum(d => d.Price * (decimal)d.Quantity),
            OrderCount = g.Select(d => d.OrderId).Distinct().Count()
        }).ToList();

        var productDetails = productList.Where(p => p.Quantity > 0).Select(p => new
        {
            p.Id,
            p.Name,
            CategoryName = p.Category.Name,
            p.Quantity,
            StockValue = (p.CostPrice ?? 0m) * (decimal)p.Quantity,
            SupplierName = p.Supplier?.Person.Name
        }).ToDictionary(p => p.Id, p => p);

        var worstSellingProducts = productDetails.Values.Select(p =>
        {
            var sale = salesStats.FirstOrDefault(s => s.ProductId == p.Id);
            return new WorstSellingProductResponse(
                p.Id,
                p.Name,
                p.CategoryName,
                sale?.QuantitySold ?? 0,
                sale?.Revenue ?? 0m,
                sale?.OrderCount ?? 0,
                p.Quantity,
                p.StockValue);
        }).OrderBy(p => p.TotalQuantitySold).ThenByDescending(p => p.CurrentStock).Take(request.TopLimit).ToList();

        return worstSellingProducts;
    }

    private static List<NeverSoldProductResponse> GetNeverSoldProduct(
        ProductPerformanceRequest request,
        IEnumerable<OrderDetailResponse> orderDetails,
        IEnumerable<ProductModel> products,
        IEnumerable<StockMovementModel> stockMovements)
    {
        var orderDetailList = orderDetails.ToList();
        var productList = products.ToList();
        var stockMovementList = stockMovements.ToList();

        var queryable = from p in productList
                        where p.Quantity > 0
                        where orderDetailList.All(d => d.ProductId != p.Id)
                        let lastMovementDate = stockMovementList.Where(m => m.ProductId == p.Id).OrderByDescending(m => m.Date)
                            .Select(m => m.Date).FirstOrDefault()
                        orderby (p.CostPrice ?? 0) * (decimal)p.Quantity descending
                        select new NeverSoldProductResponse(
                            p.Id,
                            p.Name,
                            p.Category.Name,
                            p.Supplier?.Person.Name,
                            p.Quantity,
                            (p.CostPrice ?? 0) * (decimal)p.Quantity,
                            lastMovementDate);

        return [.. queryable.Take(request.TopLimit)];
    }

    private static List<ProfitMarginResponse> GetProfitMarginsList(IEnumerable<ProductModel> products)
    {
        var productList = products.ToList();

        var rawMargins = (from p in productList
                          where p.SalesPrice > 0
                          let costPrice = p.CostPrice ?? 0m
                          let margin = (p.SalesPrice - costPrice) / p.SalesPrice * 100m
                          orderby margin descending
                          select new
                          {
                              p.Id,
                              p.Name,
                              CategoryName = p.Category.Name,
                              costPrice,
                              p.SalesPrice,
                              margin
                          }).ToList();

        var profitMargins = rawMargins.Select(p => new ProfitMarginResponse(
            p.Id,
            p.Name,
            p.CategoryName,
            p.costPrice,
            p.SalesPrice,
            p.margin,
            ClassifyMargin((double)p.margin))).ToList();

        return profitMargins;
    }

    private async Task<List<BestSellingProductResponse>> GetBestSellingProductAsync(
        ProductPerformanceRequest request,
        IEnumerable<OrderDetailResponse> orderDetails,
        CancellationToken cancellationToken = default)
    {
        var orderDetailList = orderDetails.ToList();

        var salesStats = orderDetailList.GroupBy(d => d.ProductId).Select(g => new
        {
            ProductId = g.Key,
            TotalQuantitySold = g.Sum(d => d.Quantity),
            TotalRevenue = g.Sum(d => d.Price * (decimal)d.Quantity),
            OrderCount = g.Select(d => d.OrderId).Distinct().Count(),
            AveragePrice = g.Average(d => d.Price)
        }).OrderByDescending(x => x.TotalQuantitySold).Take(request.TopLimit).ToList();

        var productIds = salesStats.Select(s => s.ProductId).ToList();
        var products = await repository.Query()
            .Include(p => p.Category)
            .Where(p => productIds.Contains(p.Id))
            .Select(p => new { p.Id, ProductName = p.Name, CategoryName = p.Category.Name })
            .ToDictionaryAsync(p => p.Id, p => p, cancellationToken);

        var bestSellingProducts = salesStats.Where(s => products.ContainsKey(s.ProductId)).Select(s =>
        {
            var details = products[s.ProductId];
            return new BestSellingProductResponse(
                s.ProductId,
                details.ProductName,
                details.CategoryName,
                s.TotalQuantitySold,
                s.TotalRevenue,
                s.OrderCount,
                s.AveragePrice);
        }).ToList();

        return bestSellingProducts;
    }
}