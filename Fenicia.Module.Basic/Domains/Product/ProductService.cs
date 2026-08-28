using Fenicia.Common;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Basic;
using Fenicia.Module.Basic.Domains.Product.DTOs;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;

namespace Fenicia.Module.Basic.Domains.Product;

public class ProductService(DefaultContext db)
{
    public async Task<Pagination<List<GetAllProductResponse>>> GetAllAsync(GetAllProductQuery query, CancellationToken ct)
    {
        var request = from p in db.BasicProducts
                      join c in db.BasicProductCategories on p.CategoryId equals c.Id
                      join s in db.BasicSuppliers on p.SupplierId equals s.Id into ps
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
                          p.IsActive);

        var total = await request.CountAsync(ct);

        var products = await request.Skip((query.Page - 1) * query.PerPage).Take(query.PerPage).ToListAsync(ct);

        return new Pagination<List<GetAllProductResponse>>(products, total, query.Page, query.PerPage);
    }

    public async Task<GetProductByIdResponse?> GetByIdAsync(GetProductByIdQuery query, CancellationToken ct)
    {
        var product = await db.BasicProducts.FirstOrDefaultAsync(p => p.Id == query.Id, ct);

        if (product is null)
        {
            return null;
        }

        var category = await db.BasicProductCategories.FirstOrDefaultAsync(c => c.Id == product.CategoryId, ct);

        SupplierModel? supplier = null;
        if (product.SupplierId.HasValue)
        {
            supplier = await db.BasicSuppliers.Include(s => s.Person).FirstOrDefaultAsync(s => s.Id == product.SupplierId, ct);
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
            supplier?.Person?.Name,
            product.IsActive);
    }

    public async Task<List<GetProductsByCategoryIdResponse>> GetByCategoryIdAsync(GetProductsByCategoryIdQuery query, int page = 1, int perPage = 10, CancellationToken ct = default)
    {
        return await db.BasicProducts
            .Where(p => p.CategoryId == query.CategoryId)
            .Select(p => new GetProductsByCategoryIdResponse(
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
                p.IsActive))
            .Skip((page - 1) * perPage)
            .Take(perPage)
            .ToListAsync(ct);
    }

    public async Task<AddProductResponse> AddAsync(AddProductCommand command, CancellationToken ct)
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
            IsActive = true
        };

        db.BasicProducts.Add(product);

        await db.SaveChangesAsync(ct);

        var category = await db.BasicProductCategories.FirstOrDefaultAsync(c => c.Id == product.CategoryId, ct);

        SupplierModel? supplier = null;
        if (product.SupplierId.HasValue)
        {
            supplier = await db.BasicSuppliers.Include(s => s.Person).FirstOrDefaultAsync(s => s.Id == product.SupplierId, ct);
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
            supplier?.Person.Name,
            product.IsActive);
    }

    public async Task<UpdateProductResponse?> UpdateAsync(UpdateProductCommand command, CancellationToken ct)
    {
        var product = await db.BasicProducts.FirstOrDefaultAsync(p => p.Id == command.Id, ct);

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

        db.BasicProducts.Update(product);

        await db.SaveChangesAsync(ct);

        var category = await db.BasicProductCategories.FirstOrDefaultAsync(c => c.Id == product.CategoryId, ct);

        SupplierModel? supplier = null;
        if (product.SupplierId.HasValue)
        {
            supplier = await db.BasicSuppliers.Include(s => s.Person).FirstOrDefaultAsync(s => s.Id == product.SupplierId, ct);
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
            supplier?.Person.Name,
            product.IsActive);
    }

    public async Task DeleteAsync(DeleteProductCommand command, CancellationToken ct)
    {
        var product = await db.BasicProducts.FirstOrDefaultAsync(p => p.Id == command.Id, ct);

        if (product is null)
        {
            return;
        }

        product.Deleted = DateTime.Now;

        db.BasicProducts.Update(product);

        await db.SaveChangesAsync(ct);
    }

    public async Task<ProductPerformanceResponse> GetPerformanceAsync(GetProductPerformanceQuery query, CancellationToken ct)
    {
        var startDate = DateTime.UtcNow.AddDays(-query.Days);
        var endDate = DateTime.UtcNow;

        var products = db.BasicProducts.Include(p => p.Category).Include(p => p.Supplier).ThenInclude(s => s != null ? s.Person : null);

        var orderDetails = db.BasicOrderDetails.Include(d => d.Order).Where(d => d.Order.SaleDate >= startDate && d.Order.SaleDate <= endDate);
        var stockMovements = db.BasicStockMovements.Where(m => m.Date >= startDate && m.Date <= endDate);

        var bestSellingProducts = await GetBestSellingProductAsync(query, orderDetails, ct);
        var worstSellingProducts = await GetWorstSellingProductAsync(query, orderDetails, products, ct);
        var profitMargins = await GetProfitMarginsListAsync(products, ct);
        var neverSoldProducts = await GetNeverSoldProductAsync(query, orderDetails, products, stockMovements, ct);

        return new ProductPerformanceResponse
        {
            BestSellingProducts = bestSellingProducts,
            WorstSellingProducts = worstSellingProducts,
            ProfitMargins = profitMargins,
            NeverSoldProducts = neverSoldProducts
        };
    }

    private async Task<List<NeverSoldProductResponse>> GetNeverSoldProductAsync(GetProductPerformanceQuery query, IQueryable<OrderDetailModel> orderDetails, IIncludableQueryable<ProductModel, PersonModel?> products, IQueryable<StockMovementModel> stockMovements, CancellationToken ct)
    {
        var queryable = from p in products
                        where p.Quantity > 0
                        where !(from d in orderDetails select d.ProductId).Contains(p.Id)
                        let lastMovementDate = (from m in stockMovements where m.ProductId == p.Id orderby m.Date descending select m.Date).FirstOrDefault()
                        orderby (p.CostPrice ?? 0) * (decimal)p.Quantity descending
                        select new NeverSoldProductResponse(p.Id, p.Name, p.Category.Name, p.Supplier != null ? p.Supplier.Person.Name : null, p.Quantity, (p.CostPrice ?? 0) * (decimal)p.Quantity, lastMovementDate);

        return await queryable.Take(query.TopLimit).ToListAsync(ct);
    }

    private async Task<List<ProfitMarginResponse>> GetProfitMarginsListAsync(IIncludableQueryable<ProductModel, PersonModel?> products, CancellationToken ct)
    {
        var rawMargins = await (from p in products
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
                                }).ToListAsync(ct);

        var profitMargins = rawMargins.Select(p => new ProfitMarginResponse(p.Id, p.Name, p.CategoryName, p.costPrice, p.SalesPrice, p.margin, ClassifyMargin((double)p.margin))).ToList();

        return profitMargins;
    }

    private async Task<List<WorstSellingProductResponse>> GetWorstSellingProductAsync(GetProductPerformanceQuery query, IQueryable<OrderDetailModel> orderDetails, IIncludableQueryable<ProductModel, PersonModel?> products, CancellationToken ct)
    {

        var salesStats = await orderDetails.GroupBy(d => d.ProductId).Select(g => new { ProductId = g.Key, QuantitySold = g.Sum(d => d.Quantity), Revenue = g.Sum(d => d.Price * (decimal)d.Quantity), OrderCount = g.Select(d => d.OrderId).Distinct().Count() }).ToListAsync(ct);

        var productDetails = await products.Where(p => p.Quantity > 0).Select(p => new
        {
            p.Id,
            p.Name,
            CategoryName = p.Category.Name,
            p.Quantity,
            StockValue = (p.CostPrice ?? 0m) * (decimal)p.Quantity,
            SupplierName = p.Supplier != null ? p.Supplier.Person.Name : null
        }).ToDictionaryAsync(p => p.Id, p => p, ct);

        var worstSellingProducts = productDetails.Values.Select(p =>
        {
            var sale = salesStats.FirstOrDefault(s => s.ProductId == p.Id);
            return new WorstSellingProductResponse(p.Id, p.Name, p.CategoryName, sale != null ? sale.QuantitySold : 0, sale != null ? sale.Revenue : 0m, sale != null ? sale.OrderCount : 0, p.Quantity, p.StockValue);
        }).OrderBy(p => p.TotalQuantitySold).ThenByDescending(p => p.CurrentStock).Take(query.TopLimit).ToList();

        return worstSellingProducts;
    }

    private async Task<List<BestSellingProductResponse>> GetBestSellingProductAsync(GetProductPerformanceQuery query, IQueryable<OrderDetailModel> orderDetails, CancellationToken ct)
    {

        var salesStats = await orderDetails.GroupBy(d => d.ProductId).Select(g => new
        {
            ProductId = g.Key,
            TotalQuantitySold = g.Sum(d => d.Quantity),
            TotalRevenue = g.Sum(d => d.Price * (decimal)d.Quantity),
            OrderCount = g.Select(d => d.OrderId).Distinct().Count(),
            AveragePrice = g.Average(d => d.Price)
        }).OrderByDescending(x => x.TotalQuantitySold).Take(query.TopLimit).ToListAsync(ct);

        var productDetails = await db.BasicProducts.Include(p => p.Category).Where(p => salesStats.Select(s => s.ProductId).Contains(p.Id)).Select(p => new { p.Id, ProductName = p.Name, CategoryName = p.Category.Name }).ToDictionaryAsync(p => p.Id, p => p, ct);

        var bestSellingProducts = salesStats.Where(s => productDetails.ContainsKey(s.ProductId)).Select(s =>
        {
            var details = productDetails[s.ProductId];
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
