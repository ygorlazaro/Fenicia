using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Basic;
using Fenicia.Common.Enums.Basic;
using Fenicia.Common.Localization;
using Fenicia.Module.Basic.Domains.StockMovement.DTOs.Commands;
using Fenicia.Module.Basic.Domains.StockMovement.DTOs.Queries;
using Fenicia.Module.Basic.Domains.StockMovement.DTOs.Responses;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Domains.StockMovement;

public class StockMovementService(DefaultContext db)
{
    public async Task<List<GetStockMovementResponse>> GetAsync(GetStockMovementQuery query, CancellationToken ct)
    {
        var request = from m in db.BasicStockMovements
                      join c in db.BasicCustomers on m.CustomerId equals c.Id into customers
                      from c in customers.DefaultIfEmpty()
                      join s in db.BasicSuppliers on m.SupplierId equals s.Id into suppliers
                      from s in suppliers.DefaultIfEmpty()
                      join e in db.BasicEmployees on m.EmployeeId equals e.Id into employees
                      from e in employees.DefaultIfEmpty()
                      where m.Date >= query.StartDate && m.Date <= query.EndDate
                      select new GetStockMovementResponse(m.Id, m.ProductId, m.Product.Name, m.Quantity, m.Date, m.Price, m.Type, m.CustomerId, c != null && c.Person != null ? c.Person.Name : null, m.SupplierId, s != null && s.Person != null ? s.Person.Name : null, m.EmployeeId, e != null && e.Person != null ? e.Person.Name : null, m.OrderId, m.Reason);

        return await request.Skip((query.Page - 1) * query.PageSize).Take(query.PageSize).ToListAsync(ct);
    }

    public async Task<AddStockMovementResponse> AddAsync(AddStockMovementCommand command, CancellationToken ct)
    {
        var stockMovement = new StockMovementModel
        {
            Id = command.Id,
            Quantity = command.Quantity,
            Date = command.Date,
            Price = command.Price,
            Type = command.Type,
            ProductId = command.ProductId,
            CustomerId = command.CustomerId,
            SupplierId = command.SupplierId,
            EmployeeId = command.EmployeeId,
            OrderId = command.OrderId,
            Reason = command.Reason
        };

        db.BasicStockMovements.Add(stockMovement);

        var product = await db.BasicProducts.FirstOrDefaultAsync(p => p.Id == command.ProductId, ct);

        if (product is not null)
        {
            product.Quantity = command.Type switch
            {
                StockMovementType.In => product.Quantity += command.Quantity,
                StockMovementType.Out => product.Quantity -= command.Quantity,
                _ => throw new ArgumentOutOfRangeException(nameof(command.Type), ExceptionMessages.InvalidRequest)
            };

            db.BasicProducts.Update(product);
        }

        await db.SaveChangesAsync(ct);

        return new AddStockMovementResponse(stockMovement.Id, stockMovement.ProductId, stockMovement.Quantity, stockMovement.Date, stockMovement.Price, stockMovement.Type, stockMovement.CustomerId, stockMovement.SupplierId, stockMovement.EmployeeId, stockMovement.OrderId, stockMovement.Reason);
    }

    public async Task<UpdateStockMovementResponse?> UpdateAsync(UpdateStockMovementCommand command, CancellationToken ct)
    {
        var stockMovement = await db.BasicStockMovements.Include(s => s.Product).FirstOrDefaultAsync(s => s.Id == command.Id, ct);

        if (stockMovement is null)
        {
            return null;
        }

        stockMovement.Date = command.Date;
        stockMovement.Type = command.Type;
        stockMovement.ProductId = command.ProductId;
        stockMovement.CustomerId = command.CustomerId;
        stockMovement.Quantity = command.Quantity;
        stockMovement.Price = command.Price;
        stockMovement.SupplierId = command.SupplierId;
        stockMovement.EmployeeId = command.EmployeeId;
        stockMovement.OrderId = command.OrderId;
        stockMovement.Reason = command.Reason;

        db.BasicStockMovements.Update(stockMovement);

        await db.SaveChangesAsync(ct);

        return new UpdateStockMovementResponse(stockMovement.Id, stockMovement.ProductId, stockMovement.Quantity, stockMovement.Date, stockMovement.Price, stockMovement.Type, stockMovement.CustomerId, stockMovement.SupplierId, stockMovement.EmployeeId, stockMovement.OrderId, stockMovement.Reason);
    }

    public async Task<StockMovementDashboardResponse> GetDashboardAsync(GetStockMovementDashboardQuery query, CancellationToken ct)
    {
        var startDate = DateTime.UtcNow.AddDays(-query.Days);
        var endDate = DateTime.UtcNow;

        var movements = db.BasicStockMovements.Where(m => m.Date >= startDate && m.Date <= endDate);

        var history = await GetStockMovementHistoryAsync(movements, ct);
        var monthlyInOut = await GetMonthlyInOutAsync(movements, ct);
        var topMovedProducts = await GetTopMovedProductAsync(query, movements, ct);
        var turnoverRates = await GetStockTurnoverAsync(query, movements, ct);

        return new StockMovementDashboardResponse
        {
            History = history,
            MonthlyInOut = monthlyInOut,
            TopMovedProducts = topMovedProducts,
            TurnoverRates = turnoverRates
        };
    }

    private async Task<List<StockTurnoverResponse>> GetStockTurnoverAsync(GetStockMovementDashboardQuery query, IQueryable<StockMovementModel> movements, CancellationToken ct)
    {
        var productOutMovements = from m in movements where m.Type == StockMovementType.Out group m by m.ProductId into g select new { ProductId = g.Key, TotalSold = (int?)g.Sum(x => x.Quantity) };

        var request = from p in db.BasicProducts
                      where p.Quantity > 0
                      join m in productOutMovements on p.Id equals m.ProductId into gj
                      from m in gj.DefaultIfEmpty()
                      let totalSold = m.TotalSold ?? 0
                      let turnoverRate = p.Quantity > 0 ? totalSold / p.Quantity : 0
                      orderby turnoverRate descending
                      select new
                      {
                          p.Id,
                          p.Name,
                          CategoryName = p.Category.Name,
                          p.Quantity,
                          totalSold,
                          turnoverRate
                      };

        var data = await request.Take(query.TopLimit).ToListAsync(ct);

        return data.Select(x => new StockTurnoverResponse(x.Id, x.Name, x.CategoryName, x.Quantity, x.totalSold, x.turnoverRate, ClassifyTurnover(x.turnoverRate))).ToList();
    }

    private async Task<List<TopMovedProductResponse>> GetTopMovedProductAsync(GetStockMovementDashboardQuery query, IQueryable<StockMovementModel> movements, CancellationToken ct)
    {
        var request = from m in movements group m by new { m.ProductId, ProductName = m.Product.Name, CategoryName = m.Product.Category.Name } into g orderby g.Sum(x => x.Quantity) descending select new TopMovedProductResponse(g.Key.ProductId, g.Key.ProductName, g.Key.CategoryName, g.Sum(x => x.Quantity), g.Sum(x => x.Price ?? 0), g.Count());

        return await request.Take(query.TopLimit).ToListAsync(ct);
    }

    private async Task<List<MonthlyInOutResponse>> GetMonthlyInOutAsync(IQueryable<StockMovementModel> movements, CancellationToken ct)
    {
        var query = from m in movements
                    where m.Date != null
                    group m by new { m.Date!.Value.Year, m.Date.Value.Month }
                    into g
                    orderby g.Key.Year, g.Key.Month
                    select new
                    {
                        g.Key.Year,
                        g.Key.Month,
                        TotalIn = g.Where(x => x.Type == StockMovementType.In).Sum(x => x.Quantity),
                        TotalOut = g.Where(x => x.Type == StockMovementType.Out).Sum(x => x.Quantity),
                        TotalInValue = g.Where(x => x.Type == StockMovementType.In).Sum(x => x.Price ?? 0),
                        TotalOutValue = g.Where(x => x.Type == StockMovementType.Out).Sum(x => x.Price ?? 0)
                    };

        var data = await query.ToListAsync(ct);

        return data.Select(x => new MonthlyInOutResponse($"{x.Month:D2}/{x.Year}", x.TotalIn, x.TotalOut, x.TotalInValue, x.TotalOutValue)).ToList();
    }

    private async Task<List<StockMovementHistoryResponse>> GetStockMovementHistoryAsync(IQueryable<StockMovementModel> movements, CancellationToken ct)
    {
        var request = from m in movements
                      join c in db.BasicCustomers on m.CustomerId equals c.Id into customers
                      from c in customers.DefaultIfEmpty()
                      join s in db.BasicSuppliers on m.SupplierId equals s.Id into suppliers
                      from s in suppliers.DefaultIfEmpty()
                      orderby m.Date descending
                      select new StockMovementHistoryResponse(m.Id, m.ProductId, m.Product.Name, m.Quantity, m.Date!.Value, m.Price ?? 0, m.Type.ToString(), m.Reason, c != null && c.Person != null ? c.Person.Name : null, s != null && s.Person != null ? s.Person.Name : null);

        return await request.ToListAsync(ct);
    }

    private string ClassifyTurnover(double rate)
    {
        return rate switch
        {
            >= 2 => "High",
            >= 1 => "Medium",
            >= 0.5 => "Low",
            _ => "Very Low"
        };
    }
}
