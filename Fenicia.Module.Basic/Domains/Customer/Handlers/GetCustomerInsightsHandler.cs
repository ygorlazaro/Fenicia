using Fenicia.Common.Data.Contexts;
using Fenicia.Module.Basic.Domains.Customer.Queries;
using Fenicia.Module.Basic.Domains.Customer.Responses;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Domains.Customer.Handlers;

public class GetCustomerInsightsHandler(DefaultContext db)
{
    public async Task<CustomerInsightsResponse> Handle(GetCustomerInsightsQuery query, CancellationToken ct)
    {
        var summary = await GetSummaryAsync(ct);
        var topCustomers = await GetTopCustomersAsync(query.TopLimit,
            ct);
        var recentOrders = await GetRecentOrdersAsync(query.TopLimit,
            ct);
        var atRiskCustomers = await GetAtRiskCustomersAsync(query,
            ct);

        return new CustomerInsightsResponse
        {
            Summary = summary,
            TopCustomers = topCustomers,
            RecentOrders = recentOrders,
            AtRiskCustomers = atRiskCustomers
        };
    }

    private async Task<List<CustomerRiskAlertResponse>> GetAtRiskCustomersAsync(GetCustomerInsightsQuery query, CancellationToken ct)
    {
        var now = DateTime.UtcNow;

        var request = from o in db.BasicOrders
                      group o by o.CustomerId
                      into g
                      select new CustomerRiskAlertResponse(
                          g.Key,
                          g.First().Customer.Person.Name,
                          g.Count(),
                          g.Max(o => o.SaleDate),
                          (now - g.Max(o => o.SaleDate)).Days,
                          g.Sum(o => o.TotalAmount),
                          (now - g.Max(o => o.SaleDate)).Days >= query.RiskThresholdDays * 2 ? "High" :
                          (now - g.Max(o => o.SaleDate)).Days >= query.RiskThresholdDays ? "Medium" : "Low"
                      );

        return await request.ToListAsync(ct);
    }

    private async Task<List<CustomerRecentOrdersResponse>> GetRecentOrdersAsync(int topLimit, CancellationToken ct)
    {
        var request = from o in db.BasicOrders
                      orderby o.SaleDate descending
                      select new CustomerRecentOrdersResponse(
                          o.Id,
                          o.CustomerId,
                          o.Customer.Person.Name,
                          o.TotalAmount,
                          o.SaleDate,
                          o.Status.ToString(),
                          o.Details.Sum(d => (int)d.Quantity)
                          );

        return await request
            .Take(topLimit)
            .ToListAsync(ct);
    }

    private async Task<List<CustomerOrderHistoryResponse>> GetTopCustomersAsync(int topLimit, CancellationToken ct)
    {
        var request = from o in db.BasicOrders
                      group o by new { o.CustomerId, CustomerName = o.Customer.Person.Name }
                      into g
                      select new CustomerOrderHistoryResponse(
                          g.Key.CustomerId,
                          g.Key.CustomerName,
                          g.Count(),
                          g.Sum(o => o.TotalAmount),
                          g.Sum(o => o.Details.Sum(d => (int)d.Quantity)),
                          g.Min(o => o.SaleDate),
                          g.Max(o => o.SaleDate),
                          g.Any() ? g.Sum(o => o.TotalAmount) / g.Count() : 0
                      );

        return await request
            .OrderByDescending(c => c.TotalSpent)
            .Take(topLimit)
            .ToListAsync(ct);
    }

    private async Task<CustomerSummaryResponse> GetSummaryAsync(CancellationToken ct)
    {
        var totalCustomers = await db.BasicCustomers.CountAsync(ct);
        var totalOrders = await db.BasicOrders.CountAsync(ct);
        var totalRevenue = await db.BasicOrders.SumAsync(o => o.TotalAmount,
            ct);
        var averageOrderValue = await db.BasicOrders.AverageAsync(o => o.TotalAmount,
            ct);
        var averageCustomerLifetimeValue = await db.BasicOrders
            .GroupBy(o => o.CustomerId)
            .AverageAsync(g => g.Sum(o => o.TotalAmount),
                ct);

        var summary = new CustomerSummaryResponse
        {
            TotalCustomers = totalCustomers,
            TotalOrders = totalOrders,
            TotalRevenue = totalRevenue,
            AverageOrderValue = averageOrderValue,
            AverageCustomerLifetimeValue = averageCustomerLifetimeValue
        };
        return summary;
    }
}
