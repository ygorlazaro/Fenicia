using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Basic;
using Fenicia.Common.Data.Repositories;
using Fenicia.Common.Enums.Auth;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Domains.Order;

public class OrderRepository(DefaultContext context) : Repository<OrderModel>(context), IOrderRepository
{
    public Task<OrderModel?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return DbSet
            .Include(o => o.Customer).ThenInclude(c => c.Person)
            .Include(o => o.Details).ThenInclude(d => d.Product)
            .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<OrderModel>> GetRecentOrdersAsync(
        int page = 1,
        int perPage = 10,
        CancellationToken cancellationToken = default)
    {
        return await DbSet
            .OrderByDescending(o => o.SaleDate)
            .Skip((page - 1) * perPage)
            .Take(perPage)
            .ToListAsync(cancellationToken);
    }

    public Task<List<Guid>> GetRecentOrderIdsAsync(
        int page = 1,
        int perPage = 10,
        CancellationToken cancellationToken = default)
    {
        return DbSet
            .OrderByDescending(o => o.SaleDate)
            .Skip((page - 1) * perPage)
            .Take(perPage)
            .Select(o => o.Id)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<OrderModel>> GetAnalyticsOrdersAsync(
        DateTime startDate,
        DateTime endDate,
        CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(o => o.Customer).ThenInclude(c => c.Person)
            .Include(o => o.Details)
            .Where(o => o.SaleDate >= startDate && o.SaleDate <= endDate)
            .ToListAsync(cancellationToken);
    }

    public Task<decimal> GetTotalRevenueAsync(CancellationToken cancellationToken = default)
    {
        return DbSet.SumAsync(o => o.TotalAmount, cancellationToken);
    }

    public Task<decimal> GetTotalCostAsync(CancellationToken cancellationToken = default)
    {
        return DbSet.SumAsync(o => o.Details.Sum(d => d.Price * (decimal)d.Quantity * 0.7m), cancellationToken);
    }

    public Task<int> GetTotalOrdersCountAsync(CancellationToken cancellationToken = default)
    {
        return DbSet.CountAsync(cancellationToken);
    }

    public Task<List<DateTime>> GetOrderDatesAsync(CancellationToken cancellationToken = default)
    {
        return DbSet
            .OrderBy(o => o.SaleDate)
            .Select(o => o.SaleDate.Date)
            .Distinct()
            .ToListAsync(cancellationToken);
    }

    public async Task<List<DateTime>> GetOrderWeeksAsync(CancellationToken cancellationToken = default)
    {
        var dates = await DbSet
            .OrderBy(o => o.SaleDate)
            .Select(o => o.SaleDate.Date)
            .Distinct()
            .ToListAsync(cancellationToken);

        var weekStarts = new List<DateTime>();
        foreach (var weekStart in dates.Select(date => date.AddDays(-(int)date.DayOfWeek)).Where(weekStart => weekStarts.Count == 0 || weekStart > weekStarts[^1]))
        {
            weekStarts.Add(weekStart);
        }

        return weekStarts;
    }

    public Task<decimal> GetTodayRevenueAsync(CancellationToken cancellationToken = default)
    {
        var today = DateTime.UtcNow.Date;
        return DbSet
            .Where(o => o.SaleDate.Date == today)
            .SumAsync(o => o.TotalAmount, cancellationToken);
    }

    public Task<int> GetTodayOrdersCountAsync(CancellationToken cancellationToken = default)
    {
        var today = DateTime.UtcNow.Date;
        return DbSet.CountAsync(o => o.SaleDate.Date == today, cancellationToken);
    }

    public Task<decimal> GetWeekRevenueAsync(CancellationToken cancellationToken = default)
    {
        var weekStart = DateTime.UtcNow.Date.AddDays(-(int)DateTime.UtcNow.DayOfWeek);
        return DbSet
            .Where(o => o.SaleDate.Date >= weekStart)
            .SumAsync(o => o.TotalAmount, cancellationToken);
    }

    public Task<int> GetWeekOrdersCountAsync(CancellationToken cancellationToken = default)
    {
        var weekStart = DateTime.UtcNow.Date.AddDays(-(int)DateTime.UtcNow.DayOfWeek);
        return DbSet.CountAsync(o => o.SaleDate.Date >= weekStart, cancellationToken);
    }

    public Task<decimal> GetMonthRevenueAsync(CancellationToken cancellationToken = default)
    {
        var monthStart = new DateTime(DateTime.UtcNow.Date.Year, DateTime.UtcNow.Date.Month, 1);
        return DbSet
            .Where(o => o.SaleDate.Date >= monthStart)
            .SumAsync(o => o.TotalAmount, cancellationToken);
    }

    public Task<int> GetMonthOrdersCountAsync(CancellationToken cancellationToken = default)
    {
        var monthStart = new DateTime(DateTime.UtcNow.Date.Year, DateTime.UtcNow.Date.Month, 1);
        return DbSet.CountAsync(o => o.SaleDate.Date >= monthStart, cancellationToken);
    }

    public Task<decimal> GetLastMonthRevenueAsync(CancellationToken cancellationToken = default)
    {
        var monthStart = new DateTime(DateTime.UtcNow.Date.Year, DateTime.UtcNow.Date.Month, 1);
        var lastMonthStart = monthStart.AddMonths(-1);
        var lastMonthEnd = monthStart.AddDays(-1);
        return DbSet
            .Where(o => o.SaleDate.Date >= lastMonthStart && o.SaleDate.Date <= lastMonthEnd)
            .SumAsync(o => o.TotalAmount, cancellationToken);
    }

    public Task<decimal> GetPendingAmountAsync(CancellationToken cancellationToken = default)
    {
        return DbSet
            .Where(o => o.Status == OrderStatus.Pending)
            .SumAsync(o => o.TotalAmount, cancellationToken);
    }

    public Task<int> GetPendingOrdersCountAsync(CancellationToken cancellationToken = default)
    {
        return DbSet.CountAsync(o => o.Status == OrderStatus.Pending, cancellationToken);
    }

    public Task<decimal> GetApprovedAmountAsync(CancellationToken cancellationToken = default)
    {
        return DbSet
            .Where(o => o.Status == OrderStatus.Approved)
            .SumAsync(o => o.TotalAmount, cancellationToken);
    }

    public Task<int> GetApprovedOrdersCountAsync(CancellationToken cancellationToken = default)
    {
        return DbSet.CountAsync(o => o.Status == OrderStatus.Approved, cancellationToken);
    }

    public Task<List<OrderModel>> GetRecentOrdersAsync(
        int topLimit,
        CancellationToken cancellationToken = default)
    {
        return DbSet
            .Include(o => o.Customer).ThenInclude(c => c.Person)
            .Include(o => o.Details)
            .OrderByDescending(o => o.SaleDate)
            .Take(topLimit * 2)
            .ToListAsync(cancellationToken);
    }

    public Task<List<OrderModel>> GetTopCustomerOrdersAsync(CancellationToken cancellationToken = default)
    {
        return DbSet
            .Include(o => o.Customer).ThenInclude(c => c.Person)
            .Include(o => o.Details)
            .ToListAsync(cancellationToken);
    }

    public Task<List<OrderModel>> GetAtRiskOrdersAsync(CancellationToken cancellationToken = default)
    {
        return DbSet
            .Include(o => o.Customer).ThenInclude(c => c.Person)
            .ToListAsync(cancellationToken);
    }

    public Task<List<OrderModel>> GetEmployeePerformanceOrdersAsync(
        DateTime startDate,
        DateTime endDate,
        CancellationToken cancellationToken = default)
    {
        return DbSet
            .Include(o => o.Employee).ThenInclude(e => e!.Person)
            .Include(o => o.Employee).ThenInclude(e => e!.Position)
            .Where(o => o.SaleDate >= startDate && o.SaleDate <= endDate)
            .ToListAsync(cancellationToken);
    }
}