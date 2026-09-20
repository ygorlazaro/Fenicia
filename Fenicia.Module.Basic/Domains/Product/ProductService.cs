using System.Linq.Expressions;
using Fenicia.Common;
using Fenicia.Common.Data.Models.Basic;
using Fenicia.Common.DTOs.Basic.DataSource;
using Fenicia.Common.DTOs.Basic.Inventory;
using Fenicia.Common.DTOs.Basic.Product;
using Fenicia.Module.Basic.Domains.OrderDetail.Interfaces;
using Fenicia.Module.Basic.Domains.Product.Interfaces;
using Fenicia.Module.Basic.Domains.StockMovement.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Domains.Product;

public sealed class ProductService(
    IProductRepository repository,
    IOrderDetailService orderDetailService,
    IStockMovementService stockMovementService) : IProductService
{
    public ProductService()
        : this(null!, null!, null!)
    {
    }

    public async Task<Pagination<List<GetAllProductResponse>>> GetAllAsync(
        GetAllProductQuery query,
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

        var response = products.Select(p => new GetAllProductResponse(
            p.Id,
            p.Name,
            p.SKU,
            p.Barcode,
            p.Description,
            p.CostPrice,
            p.SalesPrice,
            p.Quantity,
            p.MinStockLevel,
            p.MaxStockLevel,
            p.ImageUrl,
            p.Weight,
            p.Dimensions,
            p.UnitOfMeasure,
            p.CategoryId,
            p.Category?.Name ?? string.Empty,
            p.SupplierId,
            p.Supplier?.Person.Name,
            p.IsActive)).ToList();

        return new Pagination<List<GetAllProductResponse>>(response, total, query.Page, query.PerPage);
    }

    public async Task<List<GetAllProductForDataSourceResponse>> GetAllForDataSourceAsync(
        CancellationToken cancellationToken = default)
    {
        var products = await repository.Query()
            .OrderBy(p => p.Name)
            .Select(p => new GetAllProductForDataSourceResponse(p.Id, p.Name))
            .ToListAsync(cancellationToken);

        return products;
    }

    public async Task<List<GetAllDashboardProductForDataSourceResponse>>
        GetAllDashboardForDataSourceAsync(CancellationToken cancellationToken = default)
    {
        var products = await repository.Query()
            .Include(p => p.Category)
            .OrderBy(p => p.Name)
            .Select(p => new GetAllDashboardProductForDataSourceResponse(
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

    public async Task<GetProductByIdResponse?> GetByIdAsync(
        GetProductByIdQuery query,
        CancellationToken cancellationToken = default)
    {
        var product = await repository.GetByIdWithDetailsAsync(query.Id, cancellationToken);

        return product is null ? null : new GetProductByIdResponse(
            product.Id,
            product.Name,
            product.SKU,
            product.Barcode,
            product.Description,
            product.CostPrice,
            product.SalesPrice,
            product.Quantity,
            product.MinStockLevel,
            product.MaxStockLevel,
            product.ImageUrl,
            product.Weight,
            product.Dimensions,
            product.UnitOfMeasure,
            product.CategoryId,
            product.Category?.Name ?? string.Empty,
            product.SupplierId,
            product.Supplier?.Person.Name,
            product.IsActive);
    }

    public async Task<List<GetProductsByCategoryIdResponse>> GetByCategoryIdAsync(
        GetProductsByCategoryIdQuery query,
        int page = 1,
        int perPage = 10,
        CancellationToken cancellationToken = default)
    {
        var products = await repository.GetByCategoryIdAsync(query.CategoryId, page, perPage, cancellationToken);

        return [.. products.Select(p => new GetProductsByCategoryIdResponse(
            p.Id,
            p.Name,
            p.SKU,
            p.Barcode,
            p.Description,
            p.CostPrice,
            p.SalesPrice,
            p.Quantity,
            p.MinStockLevel,
            p.MaxStockLevel,
            p.ImageUrl,
            p.Weight,
            p.Dimensions,
            p.UnitOfMeasure,
            p.CategoryId,
            p.Category?.Name ?? string.Empty,
            p.IsActive))];
    }

    public async Task<AddProductResponse> AddAsync(
        AddProductCommand command,
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

        return new AddProductResponse(
            insertedProduct!.Id,
            insertedProduct.Name,
            insertedProduct.SKU,
            insertedProduct.Barcode,
            insertedProduct.Description,
            insertedProduct.CostPrice,
            insertedProduct.SalesPrice,
            insertedProduct.Quantity,
            insertedProduct.MinStockLevel,
            insertedProduct.MaxStockLevel,
            insertedProduct.ImageUrl,
            insertedProduct.Weight,
            insertedProduct.Dimensions,
            insertedProduct.UnitOfMeasure,
            insertedProduct.CategoryId,
            insertedProduct.Category?.Name ?? string.Empty,
            insertedProduct.SupplierId,
            insertedProduct.Supplier?.Person.Name,
            insertedProduct.IsActive);
    }

    public async Task<UpdateProductResponse?> UpdateAsync(
        UpdateProductCommand command,
        Guid companyId,
        CancellationToken cancellationToken = default)
    {
        var product = await repository.GetByIdAsync(command.Id, cancellationToken);

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

        return new UpdateProductResponse(
            updatedProduct!.Id,
            updatedProduct.Name,
            updatedProduct.SKU,
            updatedProduct.Barcode,
            updatedProduct.Description,
            updatedProduct.CostPrice,
            updatedProduct.SalesPrice,
            updatedProduct.Quantity,
            updatedProduct.MinStockLevel,
            updatedProduct.MaxStockLevel,
            updatedProduct.ImageUrl,
            updatedProduct.Weight,
            updatedProduct.Dimensions,
            updatedProduct.UnitOfMeasure,
            updatedProduct.CategoryId,
            updatedProduct.Category?.Name ?? string.Empty,
            updatedProduct.SupplierId,
            updatedProduct.Supplier?.Person.Name,
            updatedProduct.IsActive);
    }

    public async Task DeleteAsync(
        DeleteProductCommand command,
        Guid companyId,
        CancellationToken cancellationToken = default)
    {
        await repository.DeleteAsync(command.Id, cancellationToken);
    }

    public async Task<ProductPerformanceResponse> GetPerformanceAsync(
        GetProductPerformanceQuery query,
        CancellationToken cancellationToken = default)
    {
        var startDate = DateTime.UtcNow.AddDays(-query.Days);
        var endDate = DateTime.UtcNow;

        var products = await repository.GetAllWithDetailsAsync(cancellationToken: cancellationToken);
        var productList = products.ToList();

        var orderDetails = await orderDetailService.GetByOrderDateRangeAsync(startDate, endDate, cancellationToken);
        var orderDetailList = orderDetails.ToList();

        var stockMovements = await stockMovementService.GetByDateRangeAsync(startDate, endDate, cancellationToken);
        var stockMovementList = stockMovements.ToList();

        var bestSellingProducts = await GetBestSellingProductAsync(query, orderDetailList, cancellationToken);
        var worstSellingProducts = GetWorstSellingProduct(query, orderDetailList, productList);
        var profitMargins = GetProfitMarginsList(productList);
        var neverSoldProducts = GetNeverSoldProduct(query, orderDetailList, productList, stockMovementList);

        return new ProductPerformanceResponse
        {
            BestSellingProducts = bestSellingProducts,
            WorstSellingProducts = worstSellingProducts,
            ProfitMargins = profitMargins,
            NeverSoldProducts = neverSoldProducts
        };
    }

    public Task<int> GetCountAsync(CancellationToken cancellationToken = default)
    {
        return repository.CountAsync(cancellationToken);
    }

    public Task<int> GetTotalProductsAsync(CancellationToken cancellationToken = default)
    {
        return repository.CountAsync(cancellationToken);
    }

    public Task<List<ProductModel>> GetAllWithSupplierAsync(CancellationToken cancellationToken = default)
    {
        return repository.Query()
            .Include(p => p.Supplier).ThenInclude(s => s != null ? s.Person : null)
            .ToListAsync(cancellationToken);
    }

    public Task<List<ProductModel>> GetAllForStatsAsync(CancellationToken cancellationToken = default)
    {
        return repository.Query()
            .Where(p => p.SupplierId.HasValue)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<ProductModel>> GetAllWithCategoryAsync(
        GetAllProductQuery query,
        CancellationToken cancellationToken = default)
    {
        var baseQuery = repository.Query()
            .Include(p => p.Category);

        return await baseQuery
            .OrderBy(p => p.Quantity)
            .Skip((query.Page - 1) * query.PerPage)
            .Take(query.PerPage)
            .ToListAsync(cancellationToken);
    }

    public Task<decimal> GetTotalCostPriceAsync(CancellationToken cancellationToken = default)
    {
        return repository.GetTotalCostPriceAsync(cancellationToken);
    }

    public Task<decimal> GetTotalSalesPriceAsync(CancellationToken cancellationToken = default)
    {
        return repository.GetTotalSalesPriceAsync(cancellationToken);
    }

    public Task<int> GetTotalQuantityAsync(CancellationToken cancellationToken = default)
    {
        return repository.GetTotalQuantityAsync(cancellationToken);
    }

    public Task<IEnumerable<ProductModel>> GetByCategoryWithCategoryAsync(
        Guid categoryId,
        int page = 1,
        int perPage = 10,
        CancellationToken cancellationToken = default)
    {
        return repository.GetByCategoryWithCategoryAsync(categoryId, page, perPage, cancellationToken);
    }

    public Task<decimal> GetTotalCostPriceByCategoryAsync(
        Guid categoryId,
        CancellationToken cancellationToken = default)
    {
        return repository.GetTotalCostPriceByCategoryAsync(categoryId, cancellationToken);
    }

    public Task<decimal> GetTotalSalesPriceByCategoryAsync(
        Guid categoryId,
        CancellationToken cancellationToken = default)
    {
        return repository.GetTotalSalesPriceByCategoryAsync(categoryId, cancellationToken);
    }

    public Task<int> GetTotalQuantityByCategoryAsync(
        Guid categoryId,
        CancellationToken cancellationToken = default)
    {
        return repository.GetTotalQuantityByCategoryAsync(categoryId, cancellationToken);
    }

    public Task<IEnumerable<ProductModel>> GetByIdWithCategoryAsync(
        Guid productId,
        int page = 1,
        int perPage = 10,
        CancellationToken cancellationToken = default)
    {
        return repository.GetByIdWithCategoryAsync(productId, page, perPage, cancellationToken);
    }

    public Task<decimal> GetTotalCostPriceByProductAsync(
        Guid productId,
        CancellationToken cancellationToken = default)
    {
        return repository.GetTotalCostPriceByProductAsync(productId, cancellationToken);
    }

    public Task<decimal> GetTotalSalesPriceByProductAsync(
        Guid productId,
        CancellationToken cancellationToken = default)
    {
        return repository.GetTotalSalesPriceByProductAsync(productId, cancellationToken);
    }

    public Task<int> GetTotalQuantityByProductAsync(
        Guid productId,
        CancellationToken cancellationToken = default)
    {
        return repository.GetTotalQuantityByProductAsync(productId, cancellationToken);
    }

    public Task<List<ProductModel>> GetLowStockAsync(CancellationToken cancellationToken = default)
    {
        return repository.GetLowStockAsync(cancellationToken);
    }

    public Task<decimal> GetTotalCostValueAsync(CancellationToken cancellationToken = default)
    {
        return repository.GetTotalCostValueAsync(cancellationToken);
    }

    public Task<decimal> GetTotalSalesValueAsync(CancellationToken cancellationToken = default)
    {
        return repository.GetTotalSalesValueAsync(cancellationToken);
    }

    public Task<List<ProductModel>> GetZeroMovementCandidatesAsync(
        IEnumerable<Guid> activeProductIds,
        CancellationToken cancellationToken = default)
    {
        return repository.GetZeroMovementCandidatesAsync(activeProductIds, cancellationToken);
    }

    public Task<int> CountAsync(
        Expression<Func<ProductModel, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        return repository.CountAsync(predicate, cancellationToken);
    }

    public Task<List<(Guid CategoryId, string CategoryName, int Quantity, decimal? CostPrice)>>
        GetStockValueByCategoryAsync(CancellationToken cancellationToken = default)
    {
        return repository.GetStockValueByCategoryAsync(cancellationToken);
    }

    public Task<List<CategoryBreakdownResponse>> GetCategoryBreakdownAsync(
        CancellationToken cancellationToken = default)
    {
        return repository.Query()
            .GroupBy(p => new { p.CategoryId, CategoryName = p.Category.Name })
            .Select(g => new CategoryBreakdownResponse(
                g.Key.CategoryId,
                g.Key.CategoryName,
                g.Sum(p => (p.CostPrice ?? 0) * (decimal)p.Quantity),
                g.Sum(p => p.SalesPrice * (decimal)p.Quantity),
                g.Sum(p => p.Quantity)))
            .ToListAsync(cancellationToken);
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
        GetProductPerformanceQuery query,
        IEnumerable<OrderDetailModel> orderDetails,
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
        }).OrderBy(p => p.TotalQuantitySold).ThenByDescending(p => p.CurrentStock).Take(query.TopLimit).ToList();

        return worstSellingProducts;
    }

    private static List<NeverSoldProductResponse> GetNeverSoldProduct(
        GetProductPerformanceQuery query,
        IEnumerable<OrderDetailModel> orderDetails,
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

        return [.. queryable.Take(query.TopLimit)];
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
        GetProductPerformanceQuery query,
        IEnumerable<OrderDetailModel> orderDetails,
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
        }).OrderByDescending(x => x.TotalQuantitySold).Take(query.TopLimit).ToList();

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

    public Task<ProductModel?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return repository.GetByIdAsync(id, cancellationToken);
    }

    public Task<ProductModel?> UpdateAsync(Guid id, ProductModel product, CancellationToken cancellationToken = default)
    {
        return repository.UpdateAsync(id, product, cancellationToken);
    }

    public Task<IEnumerable<ProductModel>> GetAllWithDetailsAsync(
        int page = 1,
        int perPage = 10,
        CancellationToken cancellationToken = default)
    {
        return repository.GetAllWithDetailsAsync(page, perPage, cancellationToken);
    }
}