using Fenicia.Common;
using Fenicia.Common.Data.Models.Basic;
using Fenicia.Common.Enums.Basic;
using Fenicia.Common.Localization;
using Fenicia.Module.Basic.Domains.Product;
using Fenicia.Module.Basic.Domains.Product.DTOs;
using Fenicia.Module.Basic.Domains.StockMovement.DTOs;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Domains.StockMovement;

public class StockMovementService(
    StockMovementRepository stockMovementRepository,
    ProductService productService)
{
    public StockMovementService()
        : this(null!, null!)
    {
    }

    public async Task<List<GetStockMovementResponse>> GetAsync(GetStockMovementQuery query, CancellationToken ct)
    {
        var startDate = query.StartDate ?? DateTime.MinValue;
        var endDate = query.EndDate ?? DateTime.MaxValue;
        var movements = await stockMovementRepository.GetWithDetailsAsync(startDate, endDate, query.Page, query.PerPage, ct);

        return [.. movements.Select(m => m.MapToGetStockMovementResponse())];
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

        var product = await productService.GetByIdAsync(new GetProductByIdQuery(command.ProductId), ct);

        if (product is not null)
        {
            var newQuantity = command.Type switch
            {
                StockMovementType.In => product.Quantity + command.Quantity,
                StockMovementType.Out => product.Quantity - command.Quantity,
                _ => throw new ArgumentOutOfRangeException(nameof(command.Type), ExceptionMessages.InvalidRequest)
            };

            var updateCommand = new UpdateProductCommand(
                product.Id,
                product.Name,
                Description: product.Description,
                CostPrice: product.CostPrice,
                SalesPrice: product.SalesPrice,
                Quantity: (double)newQuantity,
                CategoryId: product.CategoryId,
                SupplierId: product.SupplierId);

            await productService.UpdateAsync(updateCommand, companyId, ct);
        }

        return stockMovement.MapToAddStockMovementResponse();
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

        return stockMovement.MapToUpdateStockMovementResponse();
    }

    public async Task<List<StockMovementModel>> GetRecentWithProductAsync(int days, int topLimit, CancellationToken ct)
    {
        var startDate = DateTime.UtcNow.AddDays(-days);
        return await stockMovementRepository.Query()
            .Include(m => m.Product)
            .Where(m => m.SupplierId.HasValue && m.Date >= startDate)
            .OrderByDescending(m => m.Date)
            .Take(topLimit)
            .ToListAsync(ct);
    }

    public async Task<StockMovementDashboardResponse> GetDashboardAsync(GetStockMovementDashboardQuery query, CancellationToken ct)
    {
        var startDate = DateTime.UtcNow.AddDays(-query.Days);
        var endDate = DateTime.UtcNow;

        var movements = await stockMovementRepository.GetWithDetailsForDashboardAsync(startDate, endDate, ct);
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

    public async Task<List<StockMovementModel>> GetByDateRangeAsync(DateTime startDate, DateTime endDate, CancellationToken ct = default)
    {
        var result = await stockMovementRepository.GetByDateRangeAsync(startDate, endDate, ct);
        return result.ToList();
    }

    public async Task<Dictionary<Guid, DateTime?>> GetLastMovementsByProductIdsAsync(IEnumerable<Guid> productIds, CancellationToken ct = default)
    {
        return await stockMovementRepository.GetLastMovementsByProductIdsAsync(productIds, ct);
    }

    private async Task<List<StockMovementHistoryResponse>> GetStockMovementHistoryAsync(IEnumerable<StockMovementModel> movements, CancellationToken ct)
    {
        var movementList = movements.ToList();

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
                          m.Customer != null && m.Customer.Person != null ? m.Customer.Person.Name : null,
                          m.Supplier != null && m.Supplier.Person != null ? m.Supplier.Person.Name : null);

        return [.. request];
    }

    private async Task<List<StockTurnoverResponse>> GetStockTurnoverAsync(GetStockMovementDashboardQuery query, IEnumerable<StockMovementModel> movements, CancellationToken ct)
    {
        var productOutMovements = movements.Where(m => m.Type == StockMovementType.Out).GroupBy(m => m.ProductId).Select(g => new { ProductId = g.Key, TotalSold = (int?)g.Sum(x => x.Quantity) });

        var products = await productService.GetAllAsync(new GetAllProductQuery(1, 10000), ct);
        var productList = products.Data.Where(p => p.Quantity > 0).ToList();

        var request = from p in productList
                      join m in productOutMovements on p.Id equals m.ProductId into gj
                      from m in gj.DefaultIfEmpty()
                      let totalSold = m.TotalSold ?? 0
                      let turnoverRate = p.Quantity > 0 ? totalSold / p.Quantity : 0
                      orderby turnoverRate descending
                      select new
                      {
                          p.Id,
                          p.Name,
                          CategoryName = p.CategoryName,
                          p.Quantity,
                          totalSold,
                          turnoverRate
                      };

        var data = request.Take(query.TopLimit).ToList();

        return [.. data.Select(x => new StockTurnoverResponse(x.Id, x.Name, x.CategoryName, x.Quantity, x.totalSold, x.turnoverRate, ClassifyTurnover(x.turnoverRate)))];
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

        return [.. data.Select(x => new MonthlyInOutResponse($"{x.Month:D2}/{x.Year}", x.TotalIn, x.TotalOut, x.TotalInValue, x.TotalOutValue))];
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
