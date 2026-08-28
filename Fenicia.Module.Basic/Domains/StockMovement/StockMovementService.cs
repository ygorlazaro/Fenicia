using Fenicia.Common.Data.Models.Basic;
using Fenicia.Common.Enums.Basic;
using Fenicia.Common.Localization;
using Fenicia.Module.Basic.Domains.StockMovement.DTOs;
using ProductRepository = Fenicia.Module.Basic.Domains.Product.ProductRepository;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Domains.StockMovement;

public class StockMovementService(
    StockMovementRepository stockMovementRepository,
    ProductRepository productRepository)
{
    public async Task<List<GetStockMovementResponse>> GetAsync(GetStockMovementQuery query, CancellationToken ct)
    {
        var startDate = query.StartDate ?? DateTime.MinValue;
        var endDate = query.EndDate ?? DateTime.MaxValue;
        var movements = await stockMovementRepository.GetWithDetailsAsync(startDate, endDate, query.Page, query.PageSize, ct);

        return movements.Select(m => new GetStockMovementResponse(
            m.Id,
            m.ProductId,
            m.Product.Name,
            m.Quantity,
            m.Date,
            m.Price,
            m.Type,
            m.CustomerId,
            m.Customer != null && m.Customer.Person != null ? m.Customer.Person.Name : null,
            m.SupplierId,
            m.Supplier != null && m.Supplier.Person != null ? m.Supplier.Person.Name : null,
            m.EmployeeId,
            m.Employee != null && m.Employee.Person != null ? m.Employee.Person.Name : null,
            m.OrderId,
            m.Reason)).ToList();
    }

    public async Task<AddStockMovementResponse> AddAsync(AddStockMovementCommand command, Guid companyId, CancellationToken ct)
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
            Reason = command.Reason,
            CompanyId = companyId
        };

        await stockMovementRepository.InsertAsync(stockMovement, ct);

        var product = await productRepository.GetByIdAsync(command.ProductId, ct);

        if (product is not null)
        {
            product.Quantity = command.Type switch
            {
                StockMovementType.In => product.Quantity += command.Quantity,
                StockMovementType.Out => product.Quantity -= command.Quantity,
                _ => throw new ArgumentOutOfRangeException(nameof(command.Type), ExceptionMessages.InvalidRequest)
            };

            await productRepository.UpdateAsync(product.Id, product, ct);
        }

        return new AddStockMovementResponse(stockMovement.Id, stockMovement.ProductId, stockMovement.Quantity, stockMovement.Date, stockMovement.Price, stockMovement.Type, stockMovement.CustomerId, stockMovement.SupplierId, stockMovement.EmployeeId, stockMovement.OrderId, stockMovement.Reason);
    }

    public async Task<UpdateStockMovementResponse?> UpdateAsync(UpdateStockMovementCommand command, Guid companyId, CancellationToken ct)
    {
        var stockMovement = await stockMovementRepository.GetByIdAsync(command.Id, ct);

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
        stockMovement.CompanyId = companyId;

        await stockMovementRepository.UpdateAsync(stockMovement.Id, stockMovement, ct);

        return new UpdateStockMovementResponse(stockMovement.Id, stockMovement.ProductId, stockMovement.Quantity, stockMovement.Date, stockMovement.Price, stockMovement.Type, stockMovement.CustomerId, stockMovement.SupplierId, stockMovement.EmployeeId, stockMovement.OrderId, stockMovement.Reason);
    }

    public async Task<StockMovementDashboardResponse> GetDashboardAsync(GetStockMovementDashboardQuery query, CancellationToken ct)
    {
        var startDate = DateTime.UtcNow.AddDays(-query.Days);
        var endDate = DateTime.UtcNow;

        var movements = await stockMovementRepository.GetByDateRangeAsync(startDate, endDate, ct);
        var movementList = movements.ToList();

        var history = await GetStockMovementHistoryAsync(movementList, ct);
        var monthlyInOut = GetMonthlyInOut(movementList);
        var topMovedProducts = await GetTopMovedProductAsync(query, movementList, ct);
        var turnoverRates = await GetStockTurnoverAsync(query, movementList, ct);

        return new StockMovementDashboardResponse
        {
            History = history,
            MonthlyInOut = monthlyInOut,
            TopMovedProducts = topMovedProducts,
            TurnoverRates = turnoverRates
        };
    }

    private async Task<List<StockMovementHistoryResponse>> GetStockMovementHistoryAsync(IEnumerable<StockMovementModel> movements, CancellationToken ct)
    {
        var movementList = movements.ToList();
        var customerIds = movementList.Where(m => m.CustomerId.HasValue).Select(m => m.CustomerId!.Value).Distinct().ToList();
        var supplierIds = movementList.Where(m => m.SupplierId.HasValue).Select(m => m.SupplierId!.Value).Distinct().ToList();

        var customers = customerIds.Any() 
            ? await stockMovementRepository.Context.BasicCustomers.Where(c => customerIds.Contains(c.Id)).Include(c => c.Person).ToDictionaryAsync(c => c.Id, c => c.Person != null ? c.Person.Name : null, ct)
            : new Dictionary<Guid, string?>();
        
        var suppliers = supplierIds.Any()
            ? await stockMovementRepository.Context.BasicSuppliers.Where(s => supplierIds.Contains(s.Id)).Include(s => s.Person).ToDictionaryAsync(s => s.Id, s => s.Person != null ? s.Person.Name : null, ct)
            : new Dictionary<Guid, string?>();

        var request = from m in movementList
                      orderby m.Date descending
                      select new StockMovementHistoryResponse(
                          m.Id,
                          m.ProductId,
                          m.Product.Name,
                          m.Quantity,
                          m.Date!.Value,
                          m.Price ?? 0,
                          m.Type.ToString(),
                          m.Reason,
                          m.CustomerId.HasValue && customers.TryGetValue(m.CustomerId.Value, out var cName) ? cName : null,
                          m.SupplierId.HasValue && suppliers.TryGetValue(m.SupplierId.Value, out var sName) ? sName : null);

        return request.ToList();
    }

    private async Task<List<StockTurnoverResponse>> GetStockTurnoverAsync(GetStockMovementDashboardQuery query, IEnumerable<StockMovementModel> movements, CancellationToken ct)
    {
        var movementList = movements.ToList();
        var productOutMovements = movementList.Where(m => m.Type == StockMovementType.Out).GroupBy(m => m.ProductId).Select(g => new { ProductId = g.Key, TotalSold = (int?)g.Sum(x => x.Quantity) });

        var products = await productRepository.Query().Where(p => p.Quantity > 0).ToListAsync(ct);

        var request = from p in products
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

        var data = request.Take(query.TopLimit).ToList();

        return data.Select(x => new StockTurnoverResponse(x.Id, x.Name, x.CategoryName, x.Quantity, x.totalSold, x.turnoverRate, ClassifyTurnover(x.turnoverRate))).ToList();
    }

    private async Task<List<TopMovedProductResponse>> GetTopMovedProductAsync(GetStockMovementDashboardQuery query, IEnumerable<StockMovementModel> movements, CancellationToken ct)
    {
        var movementList = movements.ToList();

        var request = movementList.GroupBy(m => new { m.ProductId, ProductName = m.Product.Name, CategoryName = m.Product.Category.Name })
            .OrderByDescending(g => g.Sum(x => x.Quantity))
            .Select(g => new TopMovedProductResponse(g.Key.ProductId, g.Key.ProductName, g.Key.CategoryName, g.Sum(x => x.Quantity), g.Sum(x => x.Price ?? 0), g.Count()));

        return await Task.FromResult(request.Take(query.TopLimit).ToList());
    }

    private List<MonthlyInOutResponse> GetMonthlyInOut(IEnumerable<StockMovementModel> movements)
    {
        var movementList = movements.ToList();

        var query = movementList
            .Where(m => m.Date != null)
            .GroupBy(m => new { m.Date!.Value.Year, m.Date.Value.Month })
            .OrderBy(g => g.Key.Year).ThenBy(g => g.Key.Month)
            .Select(g => new
            {
                g.Key.Year,
                g.Key.Month,
                TotalIn = g.Where(x => x.Type == StockMovementType.In).Sum(x => x.Quantity),
                TotalOut = g.Where(x => x.Type == StockMovementType.Out).Sum(x => x.Quantity),
                TotalInValue = g.Where(x => x.Type == StockMovementType.In).Sum(x => x.Price ?? 0),
                TotalOutValue = g.Where(x => x.Type == StockMovementType.Out).Sum(x => x.Price ?? 0)
            });

        var data = query.ToList();

        return data.Select(x => new MonthlyInOutResponse($"{x.Month:D2}/{x.Year}", x.TotalIn, x.TotalOut, x.TotalInValue, x.TotalOutValue)).ToList();
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
