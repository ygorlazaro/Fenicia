using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Enums.Basic;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Domains.Product.GetProductPerformance;

public class GetProductPerformanceHandler(DefaultContext context)
{
    public async Task<ProductPerformanceResponse> Handle(GetProductPerformanceQuery query, CancellationToken ct)
    {
        var startDate = DateTime.UtcNow.AddDays(-query.Days);
        var endDate = DateTime.UtcNow;

        var products = await context.BasicProducts
            .Include(p => p.Category)
            .Include(p => p.Supplier)
            .ThenInclude(s => s != null ? s.Person : null)
            .ToListAsync(ct);

        var orderDetails = await context.BasicOrderDetails
            .Include(d => d.OrderModel)
            .Where(d => d.OrderModel.SaleDate >= startDate && d.OrderModel.SaleDate <= endDate)
            .ToListAsync(ct);

        var stockMovements = await context.BasicStockMovements
            .Where(m => m.Date >= startDate && m.Date <= endDate)
            .ToListAsync(ct);

        // 1. Best Selling Products (by quantity sold)
        var bestSellingProducts = orderDetails
            .GroupBy(d => new { d.ProductId, ProductName = products.FirstOrDefault(p => p.Id == d.ProductId)?.Name ?? "Unknown", CategoryName = products.FirstOrDefault(p => p.Id == d.ProductId)?.Category?.Name ?? "Unknown" })
            .Select(g => new BestSellingProductResponse(
                g.Key.ProductId,
                g.Key.ProductName,
                g.Key.CategoryName,
                g.Sum(d => d.Quantity),
                g.Sum(d => d.Price * (decimal)d.Quantity),
                g.Select(d => d.OrderId).Distinct().Count(),
                g.Average(d => d.Price)))
            .OrderByDescending(p => p.TotalQuantitySold)
            .Take(query.TopLimit)
            .ToList();

        // 2. Worst Selling Products (low sales with high stock)
        var productSales = orderDetails
            .GroupBy(d => d.ProductId)
            .ToDictionary(
                g => g.Key,
                g => new
                {
                    QuantitySold = g.Sum(d => d.Quantity),
                    Revenue = g.Sum(d => d.Price * (decimal)d.Quantity),
                    OrderCount = g.Select(d => d.OrderId).Distinct().Count()
                });

        var worstSellingProducts = products
            .Where(p => p.Quantity > 0)
            .Select(p =>
            {
                var sales = productSales.TryGetValue(p.Id, out var s) ? s : null;
                return new WorstSellingProductResponse(
                    p.Id,
                    p.Name,
                    p.Category.Name,
                    sales?.QuantitySold ?? 0,
                    sales?.Revenue ?? 0,
                    sales?.OrderCount ?? 0,
                    p.Quantity,
                    (p.CostPrice ?? 0) * (decimal)p.Quantity);
            })
            .OrderBy(p => p.TotalQuantitySold)
            .ThenByDescending(p => p.CurrentStock)
            .Take(query.TopLimit)
            .ToList();

        // 3. Profit Margin by Product
        var profitMargins = products
            .Where(p => p.SalesPrice > 0)
            .Select(p =>
            {
                var costPrice = p.CostPrice ?? 0;
                var margin = p.SalesPrice > 0 ? ((p.SalesPrice - costPrice) / p.SalesPrice) * 100 : 0;
                var classification = ClassifyMargin((double)margin);

                return new ProfitMarginResponse(
                    p.Id,
                    p.Name,
                    p.Category.Name,
                    costPrice,
                    p.SalesPrice,
                    margin,
                    classification);
            })
            .OrderByDescending(p => p.ProfitMargin)
            .ToList();

        // 4. Products Never Sold
        var soldProductIds = orderDetails.Select(d => d.ProductId).Distinct().ToHashSet();
        var neverSoldProducts = products
            .Where(p => !soldProductIds.Contains(p.Id) && p.Quantity > 0)
            .Select(p =>
            {
                var lastMovement = stockMovements
                    .Where(m => m.ProductId == p.Id)
                    .OrderByDescending(m => m.Date)
                    .FirstOrDefault();

                return new NeverSoldProductResponse(
                    p.Id,
                    p.Name,
                    p.Category.Name,
                    p.Supplier?.Person?.Name,
                    p.Quantity,
                    (p.CostPrice ?? 0) * (decimal)p.Quantity,
                    lastMovement?.Date);
            })
            .OrderByDescending(p => p.CostValue)
            .Take(query.TopLimit)
            .ToList();

        return new ProductPerformanceResponse
        {
            BestSellingProducts = bestSellingProducts,
            WorstSellingProducts = worstSellingProducts,
            ProfitMargins = profitMargins,
            NeverSoldProducts = neverSoldProducts
        };
    }

    private static string ClassifyMargin(double margin)
    {
        if (margin >= 50) return "Excellent";
        if (margin >= 30) return "Good";
        if (margin >= 15) return "Average";
        if (margin >= 5) return "Low";
        return "Very Low";
    }
}
