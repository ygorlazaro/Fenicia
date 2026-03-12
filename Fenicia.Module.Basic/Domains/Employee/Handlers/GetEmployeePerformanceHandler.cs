using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Basic;
using Fenicia.Module.Basic.Domains.Employee.Queries;
using Fenicia.Module.Basic.Domains.Employee.Responses;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Domains.Employee.Handlers;

public class GetEmployeePerformanceHandler(DefaultContext db)
{
    public async Task<EmployeePerformanceResponse> Handle(GetEmployeePerformanceQuery query, CancellationToken ct)
    {
        var endDate = DateTime.UtcNow;
        var startDate = endDate.AddDays(-query.Days);

        var orders = db.BasicOrders
            .Where(o => o.SaleDate >= startDate && o.SaleDate <= endDate);
        
        var summary = await GetEmployeePerformanceSummaryAsync(orders, ct);
        var salesByEmployee = GetSalesByEmployeeAsync(orders);
        var ordersByEmployee = await GetOrdersByEmployeeAsync(orders, ct);
        var topPerformers = GetTopPerformerAsync(query, salesByEmployee, summary);

        return new EmployeePerformanceResponse
        {
            Summary = summary,
            SalesByEmployee = salesByEmployee,
            OrdersByEmployee = ordersByEmployee,
            TopPerformers = topPerformers
        };
    }

    private List<TopPerformerResponse> GetTopPerformerAsync(GetEmployeePerformanceQuery query, List<EmployeeSalesResponse> salesByEmployee,
        EmployeePerformanceSummaryResponse summary)
    {
        var topPerformers = salesByEmployee
            .Take(query.TopLimit)
            .Select(e =>
            {
                var performanceLevel = "Standard";
                if (e.TotalSales >= summary.AverageSalesPerEmployee * 2)
                {
                    performanceLevel = "Excellent";
                }
                else if (e.TotalSales >= summary.AverageSalesPerEmployee * (decimal)1.5)
                {
                    performanceLevel = "Very Good";
                }
                else if (e.TotalSales >= summary.AverageSalesPerEmployee)
                {
                    performanceLevel = "Good";
                }

                return new TopPerformerResponse(
                    e.EmployeeId,
                    e.EmployeeName,
                    e.PositionName,
                    e.TotalSales,
                    e.TotalOrders,
                    performanceLevel);
            })
            .ToList();
        return topPerformers;
    }

    private async Task<List<EmployeeOrderCountResponse>> GetOrdersByEmployeeAsync(IQueryable<OrderModel> orders, CancellationToken ct)
    {
        var ordersByEmployee = await orders
            .Where(o => o.EmployeeId.HasValue)
            .GroupBy(o => new
            {
                o.EmployeeId,
                EmployeeName = o.Employee!.Person.Name,
                PositionName = o.Employee.Position.Name
            })
            .Select(g => new EmployeeOrderCountResponse(
                g.Key.EmployeeId!.Value,
                g.Key.EmployeeName,
                g.Key.PositionName,
                g.Count(),
                g.Sum(o => o.TotalAmount),
                g.Min(o => o.SaleDate),
                g.Max(o => o.SaleDate)))
            .OrderByDescending(e => e.OrderCount)
            .ToListAsync(ct);
        
        return ordersByEmployee;
    }

    private List<EmployeeSalesResponse> GetSalesByEmployeeAsync(IQueryable<OrderModel> orders)
    {
        var request = from o in orders.AsEnumerable()
                      join e in db.BasicEmployees on o.EmployeeId equals e.Id
                      group o by new
                      {
                          o.EmployeeId, EmployeeName = e.Person.Name,
                          PositionName = e.Position.Name
                      }
                      into g
                      let totalSales = g.Sum(o => o.TotalAmount)
                      let totalOrders = g.Count()
                      let averageOrderValue = totalOrders > 0 ? totalSales / totalOrders : 0
                      select new EmployeeSalesResponse(
                          g.Key.EmployeeId!.Value,
                          g.Key.EmployeeName,
                          g.Key.PositionName,
                          totalSales,
                          totalOrders,
                          totalOrders > 0 ? totalSales : 0,
                          0);

        var data = request.ToList();
        
        for (var i = 0; i < data.Count; i++)
        {
            data[i] = data[i] with { Rank = i + 1 };
        }

        return data;
    }

    private async Task<EmployeePerformanceSummaryResponse> GetEmployeePerformanceSummaryAsync(
        IQueryable<OrderModel> orders,
        CancellationToken ct)
    {
        var employeesWithOrders = await orders
            .Where(o => o.EmployeeId.HasValue)
            .Select(o => o.EmployeeId!.Value)
            .Distinct()
            .CountAsync(ct);

        var totalSales = await orders
            .Where(o => o.EmployeeId.HasValue)
            .SumAsync(o => o.TotalAmount, ct);

        var totalOrders = await orders.CountAsync(o => o.EmployeeId.HasValue, ct);

        var summary = new EmployeePerformanceSummaryResponse
        {
            TotalEmployees = await db.BasicEmployees.CountAsync(ct),
            ActiveEmployees = employeesWithOrders,
            TotalSales = totalSales,
            TotalOrders = totalOrders,
            AverageSalesPerEmployee = employeesWithOrders > 0 ? totalSales / employeesWithOrders : 0,
            AverageOrdersPerEmployee = employeesWithOrders > 0 ? totalOrders / employeesWithOrders : 0
        };
        return summary;
    }
}
