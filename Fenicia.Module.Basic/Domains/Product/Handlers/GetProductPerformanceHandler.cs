using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Basic;
using Fenicia.Module.Basic.Domains.Product.DTOs.Queries;
using Fenicia.Module.Basic.Domains.Product.DTOs.Responses;
using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;

namespace Fenicia.Module.Basic.Domains.Product.Handlers;

public class GetProductPerformanceHandler(DefaultContext db) : IRequestHandler<GetProductPerformanceQuery, ProductPerformanceResponse>
{

    public async Task<ProductPerformanceResponse> Handle(GetProductPerformanceQuery query, CancellationToken ct)
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