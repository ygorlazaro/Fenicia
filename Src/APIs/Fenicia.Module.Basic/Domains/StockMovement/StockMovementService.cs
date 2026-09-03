using Fenicia.Common;
using Fenicia.Common.Data.Models.Basic;
using Fenicia.Common.Enums.Basic;
using Fenicia.Common.Exceptions;
using Fenicia.Common.Localization;
using Fenicia.Module.Basic.Domains.Product;
using Fenicia.Module.Basic.Domains.StockMovement.DTOs;
using Fenicia.Module.Basic.Domains.StockMovement.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Domains.StockMovement;

public sealed class StockMovementService(
    IStockMovementRepository stockMovementRepository,
    IProductRepository productRepository) : IStockMovementService
{
    public StockMovementService()
        : this(null!, null!)
    {
    }

    public async Task<List<GetStockMovementResponse>> GetAsync(
        GetStockMovementQuery query,
        CancellationToken cancellationToken = default)
    {
        var startDate = query.StartDate ?? DateTime.MinValue;
        var endDate = query.EndDate ?? DateTime.MaxValue;

        var baseQuery = stockMovementRepository.Query()
            .Include(m => m.Product).ThenInclude(p => p.Category)
            .Include(m => m.Customer!).ThenInclude(c => c.Person)
            .Include(m => m.Supplier!).ThenInclude(s => s.Person)
            .Include(m => m.Employee!).ThenInclude(e => e.Person)
            .Where(m => m.Date >= startDate && m.Date <= endDate);

        var filters = AdvancedQueryParser.Parse(query.Query);
        var filteredQuery = baseQuery.ApplyAdvancedQuery(filters, query.Sort);

        var movements = await filteredQuery
            .Skip((query.Page - 1) * query.PerPage)
            .Take(query.PerPage)
            .ToListAsync(cancellationToken);

        return [.. movements.Select(m => m.MapToGetStockMovementResponse())];
    }

    public async Task<AddStockMovementResponse> AddAsync(
        AddStockMovementCommand command,
        Guid companyId,
        CancellationToken cancellationToken = default)
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

        await stockMovementRepository.InsertAsync(stockMovement, cancellationToken);

        var product = await productRepository.GetByIdAsync(command.ProductId, cancellationToken);

        if (product is null)
        {
            return stockMovement.MapToAddStockMovementResponse();
        }

        var newQuantity = command.Type switch
        {
            StockMovementType.In => product.Quantity + command.Quantity,
            StockMovementType.Out => product.Quantity - command.Quantity,
            StockMovementType.None => throw new ItemNotExistsException(),
            _ => throw new ArgumentOutOfRangeException(nameof(command.Type), ExceptionMessages.InvalidRequest)
        };

        product.Quantity = newQuantity;
        await productRepository.UpdateAsync(product.Id, product, cancellationToken);

        return stockMovement.MapToAddStockMovementResponse();
    }

    public async Task<UpdateStockMovementResponse?> UpdateAsync(
        UpdateStockMovementCommand command,
        Guid companyId,
        CancellationToken cancellationToken = default)
    {
        var stockMovement = await stockMovementRepository.GetByIdAsync(command.Id, cancellationToken);

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

        await stockMovementRepository.UpdateAsync(stockMovement.Id, stockMovement, cancellationToken);

        return stockMovement.MapToUpdateStockMovementResponse();
    }

    public Task<List<StockMovementModel>> GetRecentWithProductAsync(
        int days,
        int topLimit,
        CancellationToken cancellationToken = default)
    {
        var startDate = DateTime.UtcNow.AddDays(-days);
        return stockMovementRepository.Query()
            .Include(m => m.Product)
            .Where(m => m.SupplierId.HasValue && m.Date >= startDate)
            .OrderByDescending(m => m.Date)
            .Take(topLimit)
            .ToListAsync(cancellationToken);
    }

    public async Task<StockMovementDashboardResponse> GetDashboardAsync(
        GetStockMovementDashboardQuery query,
        CancellationToken cancellationToken = default)
    {
        var startDate = DateTime.UtcNow.AddDays(-query.Days);
        var endDate = DateTime.UtcNow;

        var movements =
            await stockMovementRepository.GetWithDetailsForDashboardAsync(startDate, endDate, cancellationToken);
        var movementList = movements.ToList();

        var history = GetStockMovementHistoryAsync(movementList);
        var monthlyInOut = GetMonthlyInOut(movementList);
        var topMovedProducts = GetTopMovedProduct(query, movementList);
        var turnoverRates = await GetStockTurnoverAsync(query, movementList, cancellationToken);

        return new StockMovementDashboardResponse
        {
            History = history,
            MonthlyInOut = monthlyInOut,
            TopMovedProducts = topMovedProducts,
            TurnoverRates = turnoverRates
        };
    }

    public async Task<List<StockMovementModel>> GetByDateRangeAsync(
        DateTime startDate,
        DateTime endDate,
        CancellationToken cancellationToken = default)
    {
        var result = await stockMovementRepository.GetByDateRangeAsync(startDate, endDate, cancellationToken);
        return [.. result];
    }

    public Task<Dictionary<Guid, DateTime?>> GetLastMovementsByProductIdsAsync(
        IEnumerable<Guid> productIds,
        CancellationToken cancellationToken = default)
    {
        return stockMovementRepository.GetLastMovementsByProductIdsAsync(productIds, cancellationToken);
    }

    private static List<TopMovedProductResponse> GetTopMovedProduct(
        GetStockMovementDashboardQuery query,
        IEnumerable<StockMovementModel> movements)
    {
        var movementList = movements.ToList();

        var request = movementList.GroupBy(m => new
                { m.ProductId, ProductName = m.Product.Name, CategoryName = m.Product.Category.Name })
            .OrderByDescending(g => g.Sum(x => x.Quantity))
            .Select(g => new TopMovedProductResponse(
                g.Key.ProductId,
                g.Key.ProductName,
                g.Key.CategoryName,
                g.Sum(x => x.Quantity),
                g.Sum(x => x.Price ?? 0),
                g.Count()));

        return [.. request.Take(query.TopLimit)];
    }

    private static List<MonthlyInOutResponse> GetMonthlyInOut(IEnumerable<StockMovementModel> movements)
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

        return
        [
            .. data.Select(x => new MonthlyInOutResponse(
                $"{x.Month:D2}/{x.Year}",
                x.TotalIn,
                x.TotalOut,
                x.TotalInValue,
                x.TotalOutValue))
        ];
    }

    private static string ClassifyTurnover(double rate)
    {
        return rate switch
        {
            >= 2 => "High",
            >= 1 => "Medium",
            >= 0.5 => "Low",
            _ => "Very Low"
        };
    }

    private static List<StockMovementHistoryResponse> GetStockMovementHistoryAsync(
        IEnumerable<StockMovementModel> movements)
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
                m.Customer?.Person.Name,
                m.Supplier?.Person.Name);

        return [.. request];
    }

    private async Task<List<StockTurnoverResponse>> GetStockTurnoverAsync(
        GetStockMovementDashboardQuery query,
        IEnumerable<StockMovementModel> movements,
        CancellationToken cancellationToken = default)
    {
        var productOutMovements = movements.Where(m => m.Type == StockMovementType.Out).GroupBy(m => m.ProductId)
            .Select(g => new { ProductId = g.Key, TotalSold = (int?)g.Sum(x => x.Quantity) });

        var products = await productRepository.GetAllWithDetailsAsync(1, 10000, cancellationToken);
        var productList = products.Where(p => p.Quantity > 0).ToList();

        var request = from p in productList
            join m in productOutMovements on p.Id equals m.ProductId into gj
            from m in gj.DefaultIfEmpty()
            let totalSold = m != null ? m.TotalSold ?? 0 : 0
            let turnoverRate = p.Quantity > 0 ? totalSold / p.Quantity : 0
            let categoryName = p.Category.Name
            orderby turnoverRate descending
            select new
            {
                p.Id,
                p.Name,
                CategoryName = categoryName,
                p.Quantity,
                totalSold,
                turnoverRate
            };

        var data = request.Take(query.TopLimit).ToList();

        return
        [
            .. data.Select(x => new StockTurnoverResponse(
                x.Id,
                x.Name,
                x.CategoryName,
                x.Quantity,
                x.totalSold,
                x.turnoverRate,
                ClassifyTurnover(x.turnoverRate)))
        ];
    }
}