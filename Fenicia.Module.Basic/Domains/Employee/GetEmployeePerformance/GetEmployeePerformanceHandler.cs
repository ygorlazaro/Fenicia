using Fenicia.Common.Data.Contexts;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Domains.Employee.GetEmployeePerformance;

public class GetEmployeePerformanceHandler(DefaultContext context)
{
    public async Task<EmployeePerformanceResponse> Handle(GetEmployeePerformanceQuery query, CancellationToken ct)
    {
        var endDate = DateTime.UtcNow;
        var startDate = endDate.AddDays(-query.Days);

        var employees = await context.BasicEmployees
            .Include(e => e.PositionModel)
            .Include(e => e.PersonModel)
            .ToListAsync(ct);

        var orders = await context.BasicOrders
            .Where(o => o.SaleDate >= startDate && o.SaleDate <= endDate)
            .ToListAsync(ct);

        // 1. Summary
        var employeesWithOrders = orders
            .Where(o => o.EmployeeId.HasValue)
            .Select(o => o.EmployeeId!.Value)
            .Distinct()
            .Count();

        var totalSales = orders
            .Where(o => o.EmployeeId.HasValue)
            .Sum(o => o.TotalAmount);

        var totalOrders = orders.Count(o => o.EmployeeId.HasValue);

        var summary = new EmployeePerformanceSummaryResponse
        {
            TotalEmployees = employees.Count,
            ActiveEmployees = employeesWithOrders,
            TotalSales = totalSales,
            TotalOrders = totalOrders,
            AverageSalesPerEmployee = employeesWithOrders > 0 ? totalSales / employeesWithOrders : 0,
            AverageOrdersPerEmployee = employeesWithOrders > 0 ? totalOrders / employeesWithOrders : 0
        };

        // 2. Sales by Employee
        var salesByEmployee = orders
            .Where(o => o.EmployeeId.HasValue)
            .GroupBy(o => new { o.EmployeeId, EmployeeName = employees.FirstOrDefault(e => e.Id == o.EmployeeId)?.PersonModel.Name ?? "Unknown", PositionName = employees.FirstOrDefault(e => e.Id == o.EmployeeId)?.PositionModel?.Name ?? "Unknown" })
            .Select((g, index) => new EmployeeSalesResponse(
                g.Key.EmployeeId!.Value,
                g.Key.EmployeeName,
                g.Key.PositionName,
                g.Sum(o => o.TotalAmount),
                g.Count(),
                g.Count() > 0 ? g.Sum(o => o.TotalAmount) / g.Count() : 0,
                index + 1))
            .OrderByDescending(e => e.TotalSales)
            .ToList();

        // Update ranks
        for (var i = 0; i < salesByEmployee.Count; i++)
        {
            salesByEmployee[i] = salesByEmployee[i] with { Rank = i + 1 };
        }

        // 3. Orders by Employee
        var ordersByEmployee = orders
            .Where(o => o.EmployeeId.HasValue)
            .GroupBy(o => new { o.EmployeeId, EmployeeName = employees.FirstOrDefault(e => e.Id == o.EmployeeId)?.PersonModel.Name ?? "Unknown", PositionName = employees.FirstOrDefault(e => e.Id == o.EmployeeId)?.PositionModel?.Name ?? "Unknown" })
            .Select(g => new EmployeeOrderCountResponse(
                g.Key.EmployeeId!.Value,
                g.Key.EmployeeName,
                g.Key.PositionName,
                g.Count(),
                g.Sum(o => o.TotalAmount),
                g.Min(o => o.SaleDate),
                g.Max(o => o.SaleDate)))
            .OrderByDescending(e => e.OrderCount)
            .ToList();

        // 4. Top Performers
        var topPerformers = salesByEmployee
            .Take(query.TopLimit)
            .Select(e =>
            {
                string performanceLevel = "Standard";
                if (e.TotalSales >= summary.AverageSalesPerEmployee * 2)
                    performanceLevel = "Excellent";
                else if (e.TotalSales >= summary.AverageSalesPerEmployee * (decimal)1.5)
                    performanceLevel = "Very Good";
                else if (e.TotalSales >= summary.AverageSalesPerEmployee)
                    performanceLevel = "Good";

                return new TopPerformerResponse(
                    e.EmployeeId,
                    e.EmployeeName,
                    e.PositionName,
                    e.TotalSales,
                    e.TotalOrders,
                    performanceLevel);
            })
            .ToList();

        return new EmployeePerformanceResponse
        {
            Summary = summary,
            SalesByEmployee = salesByEmployee,
            OrdersByEmployee = ordersByEmployee,
            TopPerformers = topPerformers
        };
    }
}
