using Fenicia.Common;
using Fenicia.Common.Data.Models.Basic;
using Fenicia.Module.Basic.Domains.DataSource.DTOs;
using Fenicia.Module.Basic.Domains.Product.DTOs;
using Fenicia.Module.Basic.Domains.ProductCategory;
using Fenicia.Module.Basic.Domains.Supplier;
using Microsoft.EntityFrameworkCore;
using SalesOrderDetailRepository = Fenicia.Module.Basic.Domains.OrderDetail.OrderDetailRepository;
using StockMovementRepository = Fenicia.Module.Basic.Domains.StockMovement.StockMovementRepository;

namespace Fenicia.Module.Basic.Domains.Product;

public class ProductService(
    ProductRepository productRepository,
    ProductCategoryRepository productCategoryRepository,
    SupplierRepository supplierRepository,
    SalesOrderDetailRepository orderDetailRepository,
    StockMovementRepository stockMovementRepository)
{
    public async Task<Pagination<List<GetAllProductResponse>>> GetAllAsync(GetAllProductQuery query, CancellationToken ct)
    {
        var total = await productRepository.CountAsync(ct);

        var products = await (from p in productRepository.Query()
                              join c in productCategoryRepository.Query() on p.CategoryId equals c.Id
                              join s in supplierRepository.Query() on p.SupplierId equals s.Id into ps
                              from s in ps.DefaultIfEmpty()
                              select new GetAllProductResponse(
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
                                  c.Name,
                                  p.SupplierId,
                                  s != null ? s.Person.Name : string.Empty,
                                  p.IsActive))
            .Skip((query.Page - 1) * query.PerPage)
            .Take(query.PerPage)
            .ToListAsync(ct);

        return new Pagination<List<GetAllProductResponse>>(products, total, query.Page, query.PerPage);
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

        var category = await productCategoryRepository.GetByIdAsync(product.CategoryId, ct);

        SupplierModel? supplier = null;
        if (product.SupplierId.HasValue)
        {
            supplier = await supplierRepository.GetByIdAsync(product.SupplierId.Value, ct);
        }

        return new GetProductByIdResponse(
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
            category?.Name ?? string.Empty,
            product.SupplierId,
            supplier != null ? supplier.Person.Name : null,
            product.IsActive);
    }

    public async Task<List<GetProductsByCategoryIdResponse>> GetByCategoryIdAsync(GetProductsByCategoryIdQuery query, int page = 1, int perPage = 10, CancellationToken ct = default)
    {
        var products = await productRepository.GetByCategoryIdAsync(query.CategoryId, page, perPage, ct);

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
            p.Category.Name,
            p.IsActive))];
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

        var category = await productCategoryRepository.GetByIdAsync(product.CategoryId, ct);

        SupplierModel? supplier = null;
        if (product.SupplierId.HasValue)
        {
            supplier = await supplierRepository.GetByIdAsync(product.SupplierId.Value, ct);
        }

        return new AddProductResponse(
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
            category?.Name ?? string.Empty,
            product.SupplierId,
            supplier != null ? supplier.Person.Name : null,
            product.IsActive);
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

        var category = await productCategoryRepository.GetByIdAsync(product.CategoryId, ct);

        SupplierModel? supplier = null;
        if (product.SupplierId.HasValue)
        {
            supplier = await supplierRepository.GetByIdAsync(product.SupplierId.Value, ct);
        }

        return new UpdateProductResponse(
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
            category?.Name ?? string.Empty,
            product.SupplierId,
            supplier != null ? supplier.Person.Name : null,
            product.IsActive);
    }

    public async Task DeleteAsync(DeleteProductCommand command, Guid companyId, CancellationToken ct)
    {
        var product = await productRepository.GetByIdAsync(command.Id, ct);

        if (product is null)
        {
            return;
        }

        product.Deleted = DateTime.Now;
        product.CompanyId = companyId;

        await productRepository.UpdateAsync(product.Id, product, ct);
    }

    public async Task<ProductPerformanceResponse> GetPerformanceAsync(GetProductPerformanceQuery query, CancellationToken ct)
    {
        var startDate = DateTime.UtcNow.AddDays(-query.Days);
        var endDate = DateTime.UtcNow;

        var products = await productRepository.GetAllWithDetailsAsync(ct: ct);
        var productList = products.ToList();

        var orderDetails = await orderDetailRepository.GetByOrderDateRangeAsync(startDate, endDate, ct);
        var orderDetailList = orderDetails.ToList();

        var stockMovements = await stockMovementRepository.GetByDateRangeAsync(startDate, endDate, ct);
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
