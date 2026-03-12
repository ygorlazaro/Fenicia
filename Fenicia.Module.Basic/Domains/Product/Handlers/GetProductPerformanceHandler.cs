using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Basic;
using Fenicia.Module.Basic.Domains.Product.Queries;
using Fenicia.Module.Basic.Domains.Product.Responses;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;

namespace Fenicia.Module.Basic.Domains.Product.Handlers;

public class GetProductPerformanceHandler(DefaultContext db)
{
    public async Task<ProductPerformanceResponse> Handle(GetProductPerformanceQuery query, CancellationToken ct)
    {
        var startDate = DateTime.UtcNow.AddDays(-query.Days);
        var endDate = DateTime.UtcNow;

        var products = db.BasicProducts
            .Include(p => p.Category)
            .Include(p => p.Supplier)
            .ThenInclude(s => s != null ? s.Person : null);

        var orderDetails = db.BasicOrderDetails
            .Include(d => d.Order)
            .Where(d => d.Order.SaleDate >= startDate && d.Order.SaleDate <= endDate);
        var stockMovements = db.BasicStockMovements
            .Where(m => m.Date >= startDate && m.Date <= endDate);

        var bestSellingProducts = await GetBestSellingProductAsync(query, orderDetails, ct);
        var worstSellingProducts = await GetWorstSellingProductAsync(query, orderDetails, products, ct);
        var profitMargins = await GetProfitMarginAsync(products, ct);
        var neverSoldProducts = await GetNeverSoldProductAsync(query, orderDetails, products, stockMovements, ct);

        return new ProductPerformanceResponse
        {
            BestSellingProducts = bestSellingProducts,
            WorstSellingProducts = worstSellingProducts,
            ProfitMargins = profitMargins,
            NeverSoldProducts = neverSoldProducts
        };
    }

    private async Task<List<NeverSoldProductResponse>> GetNeverSoldProductAsync(
        GetProductPerformanceQuery query,
        IQueryable<OrderDetailModel> orderDetails,
        IIncludableQueryable<ProductModel, PersonModel?> products,
        IQueryable<StockMovementModel> stockMovements,
        CancellationToken ct)
    {
        var queryable =
            from p in products
            where p.Quantity > 0
            where !(from d in orderDetails
                    select d.ProductId).Contains(p.Id)
            let lastMovementDate =
                (from m in stockMovements
                 where m.ProductId == p.Id
                 orderby m.Date descending
                 select m.Date).FirstOrDefault()
            orderby ((p.CostPrice ?? 0) * (decimal)p.Quantity) descending
            select new NeverSoldProductResponse(
                p.Id,
                p.Name,
                p.Category.Name,
                p.Supplier != null ? p.Supplier.Person.Name : null,
                p.Quantity,
                (p.CostPrice ?? 0) * (decimal)p.Quantity,
                lastMovementDate
            );
      
        return await queryable
            .Take(query.TopLimit)
            .ToListAsync(ct);
    }

    private async Task<List<ProfitMarginResponse>> GetProfitMarginAsync(
        IIncludableQueryable<ProductModel, PersonModel?> products,
        CancellationToken ct)
    {
        var request = from p in products
                      where p.SalesPrice > 0
                      let costPrice = p.CostPrice ?? 0
                      let margin = ((p.SalesPrice - costPrice) / p.SalesPrice) * 100
                      let classification = ClassifyMargin((double)margin)
                      orderby margin descending
                      select new ProfitMarginResponse(
                          p.Id,
                          p.Name,
                          p.Category.Name,
                          costPrice,
                          p.SalesPrice,
                          margin,
                          classification);

        return await request.ToListAsync(ct);
    }

    private async Task<List<WorstSellingProductResponse>> GetWorstSellingProductAsync(
        GetProductPerformanceQuery query,
        IQueryable<OrderDetailModel> orderDetails,
        IIncludableQueryable<ProductModel, PersonModel?> products,
        CancellationToken ct)
    {
        var productSales =
            from d in orderDetails
            group d by d.ProductId
            into g
            select new
            {
                ProductId = g.Key,
                QuantitySold = g.Sum(x => x.Quantity),
                Revenue = g.Sum(x => x.Price * (decimal)x.Quantity),
                OrderCount = g.Select(x => x.OrderId).Distinct().Count()
            };

        var queryable =
            from p in products
            where p.Quantity > 0
            join s in productSales
                on p.Id equals s.ProductId into salesGroup
            from s in salesGroup.DefaultIfEmpty()
            orderby (s != null ? s.QuantitySold : 0),
                p.Quantity descending
            select new WorstSellingProductResponse(
                p.Id,
                p.Name,
                p.Category.Name,
                s != null ? s.QuantitySold : 0,
                s != null ? s.Revenue : 0,
                s != null ? s.OrderCount : 0,
                p.Quantity,
                (p.CostPrice ?? 0) * (decimal)p.Quantity
            );

        var worstSellingProducts = await queryable.OrderBy(p => p.TotalQuantitySold)
            .ThenByDescending(p => p.CurrentStock)
            .Take(query.TopLimit)
            .ToListAsync(ct);

        return worstSellingProducts;
    }

    private async Task<List<BestSellingProductResponse>> GetBestSellingProductAsync(
        GetProductPerformanceQuery query,
        IQueryable<OrderDetailModel> orderDetails,
        CancellationToken ct)
    {
        var request = from d in orderDetails
                      group d by new
                      {
                          d.ProductId,
                          ProductName = d.Product.Name,
                          CategoryName = d.Product.Category.Name
                      }
                      into g
                      select new BestSellingProductResponse(
                          g.Key.ProductId,
                          g.Key.ProductName,
                          g.Key.CategoryName,
                          g.Sum(d => d.Quantity),
                          g.Sum(d => d.Price * (decimal)d.Quantity),
                          g.Select(d => d.OrderId).Distinct().Count(),
                          g.Average(d => d.Price)
                      );

        var bestSellingProducts = await request
            .OrderByDescending(p => p.TotalQuantitySold)
            .Take(query.TopLimit)
            .ToListAsync(ct);
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