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

        var orders = await db.BasicOrders
            .Include(o => o.Employee)
                .ThenInclude(e => e!.Person)
            .Include(o => o.Employee)
                .ThenInclude(e => e!.Position)
            .Where(o => o.SaleDate >= startDate && o.SaleDate <= endDate)
            .ToListAsync(ct);
        
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

    private async Task<List<EmployeeOrderCountResponse>> GetOrdersByEmployeeAsync(IEnumerable<OrderModel> orders, CancellationToken ct)
    {
        var employees = await db.BasicEmployees
            .Include(e => e.Person)
            .Include(e => e.Position)
            .ToListAsync(ct);

        var ordersList = orders.Where(o => o.EmployeeId.HasValue).ToList();
        
        var ordersByEmployee = ordersList
            .GroupBy(o => o.EmployeeId!.Value)
            .Select(g =>
            {
                var employee = employees.First(e => e.Id == g.Key);
                return new EmployeeOrderCountResponse(
                    g.Key,
                    employee.Person.Name,
                    employee.Position.Name,
                    g.Count(),
                    g.Sum(o => o.TotalAmount),
                    g.Min(o => o.SaleDate),
                    g.Max(o => o.SaleDate));
            })
            .OrderByDescending(e => e.OrderCount)
            .ToList();
        
        return ordersByEmployee;
    }

    private List<EmployeeSalesResponse> GetSalesByEmployeeAsync(IEnumerable<OrderModel> orders)
    {
        var ordersList = orders.Where(o => o.Employee != null).ToList();

        var data = ordersList
            .GroupBy(o => o.Employee!.Id)
            .Select(g =>
            {
                var employee = g.First().Employee!;
                return new EmployeeSalesResponse(
                    employee.Id,
                    employee.Person.Name,
                    employee.Position.Name,
                    g.Sum(o => o.TotalAmount),
                    g.Count(),
                    g.Sum(o => o.TotalAmount),
                    0);
            })
            .ToList();
        
        for (var i = 0; i < data.Count; i++)
        {
            data[i] = data[i] with { Rank = i + 1 };
        }

        return data;
    }

    private async Task<EmployeePerformanceSummaryResponse> GetEmployeePerformanceSummaryAsync(
        IEnumerable<OrderModel> orders,
        CancellationToken ct)
    {
        var ordersList = orders.Where(o => o.EmployeeId.HasValue).ToList();
        
        var employeesWithOrders = ordersList
            .Select(o => o.EmployeeId!.Value)
            .Distinct()
            .Count();

        var totalSales = ordersList.Sum(o => o.TotalAmount);
        var totalOrders = ordersList.Count;

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
