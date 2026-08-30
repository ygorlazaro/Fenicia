using System.Linq;
using System.Linq.Expressions;

using Fenicia.Common;
using Fenicia.Common.Data.Models.Basic;
using Fenicia.Module.Basic.Domains.DataSource.DTOs;
using Fenicia.Module.Basic.Domains.Inventory.DTOs;
using Fenicia.Module.Basic.Domains.OrderDetail;
using Fenicia.Module.Basic.Domains.Product.DTOs;
using Fenicia.Module.Basic.Domains.ProductCategory;
using Fenicia.Module.Basic.Domains.ProductCategory.DTOs;
using Fenicia.Module.Basic.Domains.StockMovement;
using Fenicia.Module.Basic.Domains.Supplier.DTOs;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Domains.Product;

public class ProductService(
    IProductRepository productRepository,
    ProductCategoryService productCategoryService,
    OrderDetailService orderDetailService,
    StockMovementService stockMovementService)
{
    public async Task<Pagination<List<GetAllProductResponse>>> GetAllAsync(GetAllProductQuery query, CancellationToken ct)
    {
        var total = await productRepository.CountAsync(ct);

        var products = await productRepository.GetAllWithDetailsAsync(query.Page, query.PerPage, ct);

        var response = products.Select(p => p.MapToGetAllProductResponse()).ToList();

        return new Pagination<List<GetAllProductResponse>>(response, total, query.Page, query.PerPage);
    }

    public async Task<List<GetAllProductForDataSourceResponse>> GetAllForDataSourceAsync(CancellationToken ct)
    {
        var products = await productRepository.Query()
            .OrderBy(p => p.Name)
            .Select(p => new GetAllProductForDataSourceResponse(p.Id, p.Name))
            .ToListAsync(ct);

        return products;
    }

    public async Task<GetProductByIdResponse?> GetByIdAsync(GetProductByIdQuery query, CancellationToken ct)
    {
        var product = await productRepository.GetByIdWithDetailsAsync(query.Id, ct);

        if (product is null)
        {
            return null;
        }

        return product.MapToGetProductByIdResponse();
    }

    public async Task<List<GetProductsByCategoryIdResponse>> GetByCategoryIdAsync(GetProductsByCategoryIdQuery query, int page = 1, int perPage = 10, CancellationToken ct = default)
    {
        var products = await productRepository.GetByCategoryIdAsync(query.CategoryId, page, perPage, ct);

        return [.. products.Select(p => p.MapToGetProductsByCategoryIdResponse())];
    }

    public async Task<AddProductResponse> AddAsync(AddProductCommand command, Guid companyId, CancellationToken ct)
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

        await productRepository.InsertAsync(product, ct);

        var insertedProduct = await productRepository.GetByIdWithDetailsAsync(product.Id, ct);

        var category = await productCategoryService.GetByIdAsync(new GetProductCategoryByIdQuery(product.CategoryId), ct);

        var supplierName = insertedProduct?.Supplier?.Person?.Name;

        return product.MapToAddProductResponse(category?.Name ?? string.Empty, supplierName);
    }

    public async Task<UpdateProductResponse?> UpdateAsync(UpdateProductCommand command, Guid companyId, CancellationToken ct)
    {
        var product = await productRepository.GetByIdAsync(command.Id, ct);

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

        await productRepository.UpdateAsync(product.Id, product, ct);

        var updatedProduct = await productRepository.GetByIdWithDetailsAsync(product.Id, ct);

        var category = await productCategoryService.GetByIdAsync(new GetProductCategoryByIdQuery(product.CategoryId), ct);

        var supplierName = updatedProduct?.Supplier?.Person?.Name;

        return product.MapToUpdateProductResponse(category?.Name ?? string.Empty, supplierName);
    }

    public async Task DeleteAsync(DeleteProductCommand command, Guid companyId, CancellationToken ct)
    {
        await productRepository.DeleteAsync(command.Id, ct);
    }

    public async Task<ProductPerformanceResponse> GetPerformanceAsync(GetProductPerformanceQuery query, CancellationToken ct)
    {
        var startDate = DateTime.UtcNow.AddDays(-query.Days);
        var endDate = DateTime.UtcNow;

        var products = await productRepository.GetAllWithDetailsAsync(ct: ct);
        var productList = products.ToList();

        var orderDetails = await orderDetailService.GetByOrderDateRangeAsync(startDate, endDate, ct);
        var orderDetailList = orderDetails.ToList();

        var stockMovements = await stockMovementService.GetByDateRangeAsync(startDate, endDate, ct);
        var stockMovementList = stockMovements.ToList();

        var bestSellingProducts = await GetBestSellingProductAsync(query, orderDetailList, ct);
        var worstSellingProducts = await GetWorstSellingProductAsync(query, orderDetailList, productList, ct);
        var profitMargins = await GetProfitMarginsListAsync(productList, ct);
        var neverSoldProducts = await GetNeverSoldProductAsync(query, orderDetailList, productList, stockMovementList, ct);

        return new ProductPerformanceResponse
        {
            BestSellingProducts = bestSellingProducts,
            WorstSellingProducts = worstSellingProducts,
            ProfitMargins = profitMargins,
            NeverSoldProducts = neverSoldProducts
        };
    }

    public async Task<int> GetCountAsync(CancellationToken ct)
    {
        return await productRepository.CountAsync(ct);
    }

    public async Task<int> GetTotalProductsAsync(CancellationToken ct)
    {
        return await productRepository.CountAsync(ct);
    }

    public async Task<List<ProductModel>> GetAllWithSupplierAsync(CancellationToken ct)
    {
        return await productRepository.Query()
            .Include(p => p.Supplier).ThenInclude(s => s != null ? s.Person : null)
            .ToListAsync(ct);
    }

    public async Task<List<ProductModel>> GetAllForStatsAsync(CancellationToken ct)
    {
        return await productRepository.Query()
            .Where(p => p.SupplierId.HasValue)
            .ToListAsync(ct);
    }

    public async Task<IEnumerable<ProductModel>> GetAllWithCategoryAsync(int page = 1, int perPage = 10, CancellationToken ct = default)
    {
        return await productRepository.GetAllWithCategoryAsync(page, perPage, ct);
    }

    public async Task<decimal> GetTotalCostPriceAsync(CancellationToken ct)
    {
        return await productRepository.GetTotalCostPriceAsync(ct);
    }

    public async Task<decimal> GetTotalSalesPriceAsync(CancellationToken ct)
    {
        return await productRepository.GetTotalSalesPriceAsync(ct);
    }

    public async Task<int> GetTotalQuantityAsync(CancellationToken ct)
    {
        return await productRepository.GetTotalQuantityAsync(ct);
    }

    public async Task<IEnumerable<ProductModel>> GetByCategoryWithCategoryAsync(Guid categoryId, int page = 1, int perPage = 10, CancellationToken ct = default)
    {
        return await productRepository.GetByCategoryWithCategoryAsync(categoryId, page, perPage, ct);
    }

    public async Task<decimal> GetTotalCostPriceByCategoryAsync(Guid categoryId, CancellationToken ct)
    {
        return await productRepository.GetTotalCostPriceByCategoryAsync(categoryId, ct);
    }

    public async Task<decimal> GetTotalSalesPriceByCategoryAsync(Guid categoryId, CancellationToken ct)
    {
        return await productRepository.GetTotalSalesPriceByCategoryAsync(categoryId, ct);
    }

    public async Task<int> GetTotalQuantityByCategoryAsync(Guid categoryId, CancellationToken ct)
    {
        return await productRepository.GetTotalQuantityByCategoryAsync(categoryId, ct);
    }

    public async Task<IEnumerable<ProductModel>> GetByIdWithCategoryAsync(Guid productId, int page = 1, int perPage = 10, CancellationToken ct = default)
    {
        return await productRepository.GetByIdWithCategoryAsync(productId, page, perPage, ct);
    }

    public async Task<decimal> GetTotalCostPriceByProductAsync(Guid productId, CancellationToken ct)
    {
        return await productRepository.GetTotalCostPriceByProductAsync(productId, ct);
    }

    public async Task<decimal> GetTotalSalesPriceByProductAsync(Guid productId, CancellationToken ct)
    {
        return await productRepository.GetTotalSalesPriceByProductAsync(productId, ct);
    }

    public async Task<int> GetTotalQuantityByProductAsync(Guid productId, CancellationToken ct)
    {
        return await productRepository.GetTotalQuantityByProductAsync(productId, ct);
    }

    public async Task<List<ProductModel>> GetLowStockAsync(CancellationToken ct = default)
    {
        return await productRepository.GetLowStockAsync(ct);
    }

    public async Task<decimal> GetTotalCostValueAsync(CancellationToken ct)
    {
        return await productRepository.GetTotalCostValueAsync(ct);
    }

    public async Task<decimal> GetTotalSalesValueAsync(CancellationToken ct)
    {
        return await productRepository.GetTotalSalesValueAsync(ct);
    }

    public async Task<List<ProductModel>> GetZeroMovementCandidatesAsync(IEnumerable<Guid> activeProductIds, CancellationToken ct = default)
    {
        return await productRepository.GetZeroMovementCandidatesAsync(activeProductIds, ct);
    }

    public async Task<List<ProductModel>> GetOverstockCandidatesAsync(CancellationToken ct = default)
    {
        return await productRepository.GetOverstockCandidatesAsync(ct);
    }

    public async Task<int> CountAsync(Expression<Func<ProductModel, bool>> predicate, CancellationToken ct = default)
    {
        return await productRepository.CountAsync(predicate, ct);
    }

    public async Task<List<(Guid CategoryId, string CategoryName, int Quantity, decimal? CostPrice)>> GetStockValueByCategoryAsync(CancellationToken ct = default)
    {
        return await productRepository.GetStockValueByCategoryAsync(ct);
    }

    public async Task<List<CategoryBreakdownResponse>> GetCategoryBreakdownAsync(CancellationToken ct)
    {
        return await productRepository.Query()
                .GroupBy(p => new { p.CategoryId, CategoryName = p.Category.Name })
            .Select(g => new CategoryBreakdownResponse(
                g.Key.CategoryId,
                g.Key.CategoryName,
                g.Sum(p => (decimal)(p.CostPrice ?? 0) * (decimal)p.Quantity),
                g.Sum(p => p.SalesPrice * (decimal)p.Quantity),
                g.Sum(p => p.Quantity)))
            .ToListAsync(ct);
    }

    private async Task<List<NeverSoldProductResponse>> GetNeverSoldProductAsync(GetProductPerformanceQuery query, IEnumerable<OrderDetailModel> orderDetails, IEnumerable<ProductModel> products, IEnumerable<StockMovementModel> stockMovements, CancellationToken ct)
    {
        var orderDetailList = orderDetails.ToList();
        var productList = products.ToList();
        var stockMovementList = stockMovements.ToList();

        var queryable = from p in productList
                        where p.Quantity > 0
                        where !orderDetailList.Any(d => d.ProductId == p.Id)
                        let lastMovementDate = stockMovementList.Where(m => m.ProductId == p.Id).OrderByDescending(m => m.Date).Select(m => m.Date).FirstOrDefault()
                        orderby (p.CostPrice ?? 0) * (decimal)p.Quantity descending
                        select new NeverSoldProductResponse(p.Id, p.Name, p.Category.Name, p.Supplier != null ? p.Supplier.Person.Name : null, p.Quantity, (p.CostPrice ?? 0) * (decimal)p.Quantity, lastMovementDate);

        return [.. queryable.Take(query.TopLimit)];
    }

    private async Task<List<ProfitMarginResponse>> GetProfitMarginsListAsync(IEnumerable<ProductModel> products, CancellationToken ct)
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

    private async Task<List<WorstSellingProductResponse>> GetWorstSellingProductAsync(GetProductPerformanceQuery query, IEnumerable<OrderDetailModel> orderDetails, IEnumerable<ProductModel> products, CancellationToken ct)
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
            SupplierName = p.Supplier != null ? p.Supplier.Person.Name : null
        }).ToDictionary(p => p.Id, p => p);

        var worstSellingProducts = productDetails.Values.Select(p =>
    {
        var sale = salesStats.FirstOrDefault(s => s.ProductId == p.Id);
        return new WorstSellingProductResponse(p.Id, p.Name, p.CategoryName, sale != null ? sale.QuantitySold : 0, sale != null ? sale.Revenue : 0m, sale != null ? sale.OrderCount : 0, p.Quantity, p.StockValue);
    }).OrderBy(p => p.TotalQuantitySold).ThenByDescending(p => p.CurrentStock).Take(query.TopLimit).ToList();

        return worstSellingProducts;
    }

    private async Task<List<BestSellingProductResponse>> GetBestSellingProductAsync(GetProductPerformanceQuery query, IEnumerable<OrderDetailModel> orderDetails, CancellationToken ct)
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
            .ToDictionaryAsync(p => p.Id, p => p, ct);

        var bestSellingProducts = salesStats.Where(s => products.ContainsKey(s.ProductId)).Select(s =>
    {
        var details = products[s.ProductId];
        return new BestSellingProductResponse(s.ProductId, details.ProductName, details.CategoryName, s.TotalQuantitySold, s.TotalRevenue, s.OrderCount, s.AveragePrice);
    }).ToList();

        return bestSellingProducts;
    }

    private string ClassifyMargin(double margin)
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
}
