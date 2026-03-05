using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Enums.Basic;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Domains.StockMovement.GetStockMovementDashboard;

public class GetStockMovementDashboardHandler(DefaultContext context)
{
    public async Task<StockMovementDashboardResponse> Handle(GetStockMovementDashboardQuery query, CancellationToken ct)
    {
        var startDate = DateTime.UtcNow.AddDays(-query.Days);
        var endDate = DateTime.UtcNow;

        var movements = await context.BasicStockMovements
            .Include(m => m.ProductModel)
            .ThenInclude(p => p.Category)
            .Include(m => m.Customer)
            .ThenInclude(c => c != null ? c.PersonModel : null)
            .Include(m => m.Supplier)
            .ThenInclude(s => s != null ? s.Person : null)
            .Where(m => m.Date >= startDate && m.Date <= endDate)
            .ToListAsync(ct);

        // 1. Stock Movement History
        var history = movements
            .OrderByDescending(m => m.Date)
            .Select(m => new StockMovementHistoryResponse(
                m.Id,
                m.ProductId,
                m.ProductModel.Name,
                m.Quantity,
                m.Date!.Value,
                m.Price ?? 0,
                m.Type.ToString(),
                m.Reason,
                m.CustomerId.HasValue ? m.Customer!.PersonModel.Name : null,
                m.SupplierId.HasValue ? m.Supplier!.Person.Name : null))
            .ToList();

        // 2. Monthly In vs Out
        var monthlyInOut = movements
            .GroupBy(m => new { Year = m.Date!.Value.Year, Month = m.Date.Value.Month, m.Type })
            .Select(g => new MonthlyInOutResponse(
                $"{g.Key.Month:D2}/{g.Key.Year}",
                g.Key.Type == StockMovementType.In ? g.Sum(m => m.Quantity) : 0,
                g.Key.Type == StockMovementType.Out ? g.Sum(m => m.Quantity) : 0,
                g.Key.Type == StockMovementType.In ? g.Sum(m => m.Price ?? 0) : 0,
                g.Key.Type == StockMovementType.Out ? g.Sum(m => m.Price ?? 0) : 0))
            .GroupBy(m => m.Month)
            .Select(g => new MonthlyInOutResponse(
                g.Key,
                g.Sum(m => m.TotalIn),
                g.Sum(m => m.TotalOut),
                g.Sum(m => m.TotalInValue),
                g.Sum(m => m.TotalOutValue)))
            .OrderBy(m => m.Month)
            .ToList();

        // 3. Top Moved Products
        var topMovedProducts = movements
            .GroupBy(m => new { m.ProductId, ProductName = m.ProductModel.Name, CategoryName = m.ProductModel.Category.Name })
            .Select(g => new TopMovedProductResponse(
                g.Key.ProductId,
                g.Key.ProductName,
                g.Key.CategoryName,
                g.Sum(m => m.Quantity),
                g.Sum(m => m.Price ?? 0),
                g.Count()))
            .OrderByDescending(p => p.TotalMoved)
            .Take(query.TopLimit)
            .ToList();

        // 4. Stock Turnover Rate
        // Calculate turnover: Total Sold (Out movements) / Average Stock
        // For simplicity, we'll use current stock as denominator
        var products = await context.BasicProducts
            .Include(p => p.Category)
            .ToListAsync(ct);

        var productOutMovements = movements
            .Where(m => m.Type == StockMovementType.Out)
            .GroupBy(m => m.ProductId)
            .ToDictionary(g => g.Key, g => g.Sum(m => m.Quantity));

        var turnoverRates = products
            .Where(p => p.Quantity > 0)
            .Select(p =>
            {
                var totalSold = productOutMovements.TryGetValue(p.Id, out var sold) ? sold : 0;
                var turnoverRate = p.Quantity > 0 ? totalSold / p.Quantity : 0;
                var classification = ClassifyTurnover(turnoverRate);

                return new StockTurnoverResponse(
                    p.Id,
                    p.Name,
                    p.Category.Name,
                    p.Quantity,
                    totalSold,
                    turnoverRate,
                    classification);
            })
            .OrderByDescending(t => t.TurnoverRate)
            .Take(query.TopLimit)
            .ToList();

        return new StockMovementDashboardResponse
        {
            History = history,
            MonthlyInOut = monthlyInOut,
            TopMovedProducts = topMovedProducts,
            TurnoverRates = turnoverRates
        };
    }

    private static string ClassifyTurnover(double rate)
    {
        if (rate >= 2) return "High";
        if (rate >= 1) return "Medium";
        if (rate >= 0.5) return "Low";
        return "Very Low";
    }
}
