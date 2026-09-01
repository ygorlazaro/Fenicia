using System.Linq.Expressions;

using Fenicia.Common;
using Fenicia.Common.Data.Models.Basic;
using Fenicia.Module.Basic.Domains.DataSource.DTOs;
using Fenicia.Module.Basic.Domains.Inventory.DTOs;
using Fenicia.Module.Basic.Domains.OrderDetail.Interfaces;
using Fenicia.Module.Basic.Domains.Product.DTOs;
using Fenicia.Module.Basic.Domains.Product.Interfaces;
using Fenicia.Module.Basic.Domains.ProductCategory.DTOs;
using Fenicia.Module.Basic.Domains.ProductCategory.Interfaces;
using Fenicia.Module.Basic.Domains.StockMovement.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Domains.Product;

public class ProductService(
    IProductRepository productRepository,
    IProductCategoryService productCategoryService,
    IOrderDetailService orderDetailService,
    IStockMovementService stockMovementService) : IProductService
{
    public ProductService()
        : this(null!, null!, null!, null!)
    {
    }

    public virtual async Task<Pagination<List<GetAllProductResponse>>> GetAllAsync(GetAllProductQuery query, CancellationToken cancellationToken = default)
    {
        var baseQuery = productRepository.Query()
            .Include(p => p.Category)
            .Include(p => p.Supplier).ThenInclude(s => s != null ? s.Person : null);

        var filters = AdvancedQueryParser.Parse(query.Query);
        var filteredQuery = baseQuery.ApplyAdvancedQuery(filters, query.Sort);

        var total = await filteredQuery.CountAsync(cancellationToken);

        var products = await filteredQuery
            .Skip((query.Page - 1) * query.PerPage)
            .Take(query.PerPage)
            .ToListAsync(cancellationToken);

        var response = products.Select(p => p.MapToGetAllProductResponse()).ToList();

        return new Pagination<List<GetAllProductResponse>>(response, total, query.Page, query.PerPage);
    }

    public virtual async Task<List<GetAllProductForDataSourceResponse>> GetAllForDataSourceAsync(CancellationToken cancellationToken = default)
    {
        var products = await productRepository.Query()
            .OrderBy(p => p.Name)
            .Select(p => new GetAllProductForDataSourceResponse(p.Id, p.Name))
            .ToListAsync(cancellationToken);

        return products;
    }

    public virtual async Task<GetProductByIdResponse?> GetByIdAsync(GetProductByIdQuery query, CancellationToken cancellationToken = default)
    {
        var product = await productRepository.GetByIdWithDetailsAsync(query.Id, cancellationToken);

        return product?.MapToGetProductByIdResponse();
    }

    public virtual async Task<List<GetProductsByCategoryIdResponse>> GetByCategoryIdAsync(GetProductsByCategoryIdQuery query, int page = 1, int perPage = 10, CancellationToken cancellationToken = default)
    {
        var products = await productRepository.GetByCategoryIdAsync(query.CategoryId, page, perPage, cancellationToken);

        return [.. products.Select(p => p.MapToGetProductsByCategoryIdResponse())];
    }

    public virtual async Task<AddProductResponse> AddAsync(AddProductCommand command, Guid companyId, CancellationToken cancellationToken = default)
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

        await productRepository.InsertAsync(product, cancellationToken);

        var insertedProduct = await productRepository.GetByIdWithDetailsAsync(product.Id, cancellationToken);

        var category = await productCategoryService.GetByIdAsync(new GetProductCategoryByIdQuery(product.CategoryId), cancellationToken);

        var supplierName = insertedProduct?.Supplier?.Person.Name;

        return product.MapToAddProductResponse(category?.Name ?? string.Empty, supplierName);
    }

    public virtual async Task<UpdateProductResponse?> UpdateAsync(UpdateProductCommand command, Guid companyId, CancellationToken cancellationToken = default)
    {
        var product = await productRepository.GetByIdAsync(command.Id, cancellationToken);

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

        await productRepository.UpdateAsync(product.Id, product, cancellationToken);

        var updatedProduct = await productRepository.GetByIdWithDetailsAsync(product.Id, cancellationToken);

        var category = await productCategoryService.GetByIdAsync(new GetProductCategoryByIdQuery(product.CategoryId), cancellationToken);

        var supplierName = updatedProduct?.Supplier?.Person.Name;

        return product.MapToUpdateProductResponse(category?.Name ?? string.Empty, supplierName);
    }

    public virtual async Task DeleteAsync(DeleteProductCommand command, Guid companyId, CancellationToken cancellationToken = default)
    {
        await productRepository.DeleteAsync(command.Id, cancellationToken);
    }

    public virtual async Task<ProductPerformanceResponse> GetPerformanceAsync(GetProductPerformanceQuery query, CancellationToken cancellationToken = default)
    {
        var startDate = DateTime.UtcNow.AddDays(-query.Days);
        var endDate = DateTime.UtcNow;

        var products = await productRepository.GetAllWithDetailsAsync(cancellationToken: cancellationToken);
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

    public virtual async Task<int> GetCountAsync(CancellationToken cancellationToken = default)
    {
        return await productRepository.CountAsync(cancellationToken);
    }

    public virtual async Task<int> GetTotalProductsAsync(CancellationToken cancellationToken = default)
    {
        return await productRepository.CountAsync(cancellationToken);
    }

    public virtual async Task<List<ProductModel>> GetAllWithSupplierAsync(CancellationToken cancellationToken = default)
    {
        return await productRepository.Query()
            .Include(p => p.Supplier).ThenInclude(s => s != null ? s.Person : null)
            .ToListAsync(cancellationToken);
    }

    public virtual async Task<List<ProductModel>> GetAllForStatsAsync(CancellationToken cancellationToken = default)
    {
        return await productRepository.Query()
            .Where(p => p.SupplierId.HasValue)
            .ToListAsync(cancellationToken);
    }

    public virtual async Task<IEnumerable<ProductModel>> GetAllWithCategoryAsync(GetAllProductQuery query, CancellationToken cancellationToken = default)
    {
        var baseQuery = productRepository.Query()
            .Include(p => p.Category);

        var filters = AdvancedQueryParser.Parse(query.Query);
        var filteredQuery = baseQuery.ApplyAdvancedQuery(filters, query.Sort);

        return await filteredQuery
            .OrderBy(p => p.Quantity)
            .Skip((query.Page - 1) * query.PerPage)
            .Take(query.PerPage)
            .ToListAsync(cancellationToken);
    }

    public virtual async Task<decimal> GetTotalCostPriceAsync(CancellationToken cancellationToken = default)
    {
        return await productRepository.GetTotalCostPriceAsync(cancellationToken);
    }

    public virtual async Task<decimal> GetTotalSalesPriceAsync(CancellationToken cancellationToken = default)
    {
        return await productRepository.GetTotalSalesPriceAsync(cancellationToken);
    }

    public virtual async Task<int> GetTotalQuantityAsync(CancellationToken cancellationToken = default)
    {
        return await productRepository.GetTotalQuantityAsync(cancellationToken);
    }

    public virtual async Task<IEnumerable<ProductModel>> GetByCategoryWithCategoryAsync(Guid categoryId, int page = 1, int perPage = 10, CancellationToken cancellationToken = default)
    {
        return await productRepository.GetByCategoryWithCategoryAsync(categoryId, page, perPage, cancellationToken);
    }

    public virtual async Task<decimal> GetTotalCostPriceByCategoryAsync(Guid categoryId, CancellationToken cancellationToken = default)
    {
        return await productRepository.GetTotalCostPriceByCategoryAsync(categoryId, cancellationToken);
    }

    public virtual async Task<decimal> GetTotalSalesPriceByCategoryAsync(Guid categoryId, CancellationToken cancellationToken = default)
    {
        return await productRepository.GetTotalSalesPriceByCategoryAsync(categoryId, cancellationToken);
    }

    public virtual async Task<int> GetTotalQuantityByCategoryAsync(Guid categoryId, CancellationToken cancellationToken = default)
    {
        return await productRepository.GetTotalQuantityByCategoryAsync(categoryId, cancellationToken);
    }

    public virtual async Task<IEnumerable<ProductModel>> GetByIdWithCategoryAsync(Guid productId, int page = 1, int perPage = 10, CancellationToken cancellationToken = default)
    {
        return await productRepository.GetByIdWithCategoryAsync(productId, page, perPage, cancellationToken);
    }

    public virtual async Task<decimal> GetTotalCostPriceByProductAsync(Guid productId, CancellationToken cancellationToken = default)
    {
        return await productRepository.GetTotalCostPriceByProductAsync(productId, cancellationToken);
    }

    public virtual async Task<decimal> GetTotalSalesPriceByProductAsync(Guid productId, CancellationToken cancellationToken = default)
    {
        return await productRepository.GetTotalSalesPriceByProductAsync(productId, cancellationToken);
    }

    public virtual async Task<int> GetTotalQuantityByProductAsync(Guid productId, CancellationToken cancellationToken = default)
    {
        return await productRepository.GetTotalQuantityByProductAsync(productId, cancellationToken);
    }

    public virtual async Task<List<ProductModel>> GetLowStockAsync(CancellationToken cancellationToken = default)
    {
        return await productRepository.GetLowStockAsync(cancellationToken);
    }

    public virtual async Task<decimal> GetTotalCostValueAsync(CancellationToken cancellationToken = default)
    {
        return await productRepository.GetTotalCostValueAsync(cancellationToken);
    }

    public virtual async Task<decimal> GetTotalSalesValueAsync(CancellationToken cancellationToken = default)
    {
        return await productRepository.GetTotalSalesValueAsync(cancellationToken);
    }

    public virtual async Task<List<ProductModel>> GetZeroMovementCandidatesAsync(IEnumerable<Guid> activeProductIds, CancellationToken cancellationToken = default)
    {
        return await productRepository.GetZeroMovementCandidatesAsync(activeProductIds, cancellationToken);
    }

    public virtual async Task<List<ProductModel>> GetOverstockCandidatesAsync(CancellationToken cancellationToken = default)
    {
        return await productRepository.GetOverstockCandidatesAsync(cancellationToken);
    }

    public virtual async Task<int> CountAsync(Expression<Func<ProductModel, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return await productRepository.CountAsync(predicate, cancellationToken);
    }

    public virtual async Task<List<(Guid CategoryId, string CategoryName, int Quantity, decimal? CostPrice)>> GetStockValueByCategoryAsync(CancellationToken cancellationToken = default)
    {
        return await productRepository.GetStockValueByCategoryAsync(cancellationToken);
    }

    public virtual async Task<List<CategoryBreakdownResponse>> GetCategoryBreakdownAsync(CancellationToken cancellationToken = default)
    {
        return await productRepository.Query()
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

    private static List<WorstSellingProductResponse> GetWorstSellingProduct(GetProductPerformanceQuery query, IEnumerable<OrderDetailModel> orderDetails, IEnumerable<ProductModel> products)
    {
        var orderDetailList = orderDetails.ToList();
        var productList = products.ToList();

        var salesStats = orderDetailList.GroupBy(d => d.ProductId).Select(g => new { ProductId = g.Key, QuantitySold = g.Sum(d => d.Quantity), Revenue = g.Sum(d => d.Price * (decimal)d.Quantity), OrderCount = g.Select(d => d.OrderId).Distinct().Count() }).ToList();

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
            return new WorstSellingProductResponse(p.Id, p.Name, p.CategoryName, sale?.QuantitySold ?? 0, sale?.Revenue ?? 0m, sale?.OrderCount ?? 0, p.Quantity, p.StockValue);
        }).OrderBy(p => p.TotalQuantitySold).ThenByDescending(p => p.CurrentStock).Take(query.TopLimit).ToList();

        return worstSellingProducts;
    }

    private static List<NeverSoldProductResponse> GetNeverSoldProduct(GetProductPerformanceQuery query, IEnumerable<OrderDetailModel> orderDetails, IEnumerable<ProductModel> products, IEnumerable<StockMovementModel> stockMovements)
    {
        var orderDetailList = orderDetails.ToList();
        var productList = products.ToList();
        var stockMovementList = stockMovements.ToList();

        var queryable = from p in productList
                        where p.Quantity > 0
                        where orderDetailList.All(d => d.ProductId != p.Id)
                        let lastMovementDate = stockMovementList.Where(m => m.ProductId == p.Id).OrderByDescending(m => m.Date).Select(m => m.Date).FirstOrDefault()
                        orderby (p.CostPrice ?? 0) * (decimal)p.Quantity descending
                        select new NeverSoldProductResponse(p.Id, p.Name, p.Category.Name, p.Supplier?.Person.Name, p.Quantity, (p.CostPrice ?? 0) * (decimal)p.Quantity, lastMovementDate);

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

        var profitMargins = rawMargins.Select(p => new ProfitMarginResponse(p.Id, p.Name, p.CategoryName, p.costPrice, p.SalesPrice, p.margin, ClassifyMargin((double)p.margin))).ToList();

        return profitMargins;
    }

    private async Task<List<BestSellingProductResponse>> GetBestSellingProductAsync(GetProductPerformanceQuery query, IEnumerable<OrderDetailModel> orderDetails, CancellationToken cancellationToken = default)
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
        var products = await productRepository.Query()
            .Include(p => p.Category)
            .Where(p => productIds.Contains(p.Id))
            .Select(p => new { p.Id, ProductName = p.Name, CategoryName = p.Category.Name })
            .ToDictionaryAsync(p => p.Id, p => p, cancellationToken);

        var bestSellingProducts = salesStats.Where(s => products.ContainsKey(s.ProductId)).Select(s =>
    {
        var details = products[s.ProductId];
        return new BestSellingProductResponse(s.ProductId, details.ProductName, details.CategoryName, s.TotalQuantitySold, s.TotalRevenue, s.OrderCount, s.AveragePrice);
    }).ToList();

        return bestSellingProducts;
    }
}
