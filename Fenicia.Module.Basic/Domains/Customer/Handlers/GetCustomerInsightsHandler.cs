using Fenicia.Common.Data.Contexts;
using Fenicia.Module.Basic.Domains.Customer.DTOs.Queries;
using Fenicia.Module.Basic.Domains.Customer.DTOs.Responses;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Domains.Customer.Handlers;

public class GetCustomerInsightsHandler(DefaultContext db) : IRequestHandler<GetCustomerInsightsQuery, CustomerInsightsResponse>
{

    public async Task<CustomerInsightsResponse> Handle(GetCustomerInsightsQuery query, CancellationToken ct)
    {
        var summary = await GetSummaryAsync(ct);
        var topCustomers = await GetTopCustomersAsync(query.TopLimit, ct);
        var recentOrders = await GetRecentOrdersAsync(query.TopLimit, ct);
        var atRiskCustomers = await GetAtRiskCustomersAsync(query, ct);

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

        var orders = await db.BasicOrders.Include(o => o.Customer).ThenInclude(c => c.Person).ToListAsync(ct);

        var response = orders.GroupBy(o => o.CustomerId).Select(g =>
        {
            var lastOrder = g.Max(o => o.SaleDate);
            var daysSince = (now - lastOrder).Days;
            var riskLevel = daysSince >= query.RiskThresholdDays * 2 ? "High" : daysSince >= query.RiskThresholdDays ? "Medium" : "Low";

            return new CustomerRiskAlertResponse(g.Key, g.First().Customer.Person.Name, g.Count(), lastOrder, daysSince, g.Sum(o => o.TotalAmount), riskLevel);
        }).Where(c => c.DaysSinceLastOrder >= query.RiskThresholdDays).OrderByDescending(c => c.DaysSinceLastOrder).ToList();

        return response;
    }

    private async Task<List<CustomerRecentOrdersResponse>> GetRecentOrdersAsync(int topLimit, CancellationToken ct)
    {
        var orders = await db.BasicOrders.Include(o => o.Customer).ThenInclude(c => c.Person).Include(o => o.Details).OrderByDescending(o => o.SaleDate).Take(topLimit * 2).ToListAsync(ct);

        var response = orders.Take(topLimit).Select(o => new CustomerRecentOrdersResponse(o.Id, o.CustomerId, o.Customer.Person.Name, o.TotalAmount, o.SaleDate, o.Status.ToString(), o.Details.Sum(d => (int)d.Quantity))).ToList();

        return response;
    }

    private async Task<List<CustomerOrderHistoryResponse>> GetTopCustomersAsync(int topLimit, CancellationToken ct)
    {
        var orders = await db.BasicOrders.Include(o => o.Customer).ThenInclude(c => c.Person).Include(o => o.Details).ToListAsync(ct);

        var response = orders.GroupBy(o => new { o.CustomerId, CustomerName = o.Customer.Person.Name }).Select(g => new CustomerOrderHistoryResponse(g.Key.CustomerId, g.Key.CustomerName, g.Count(), g.Sum(o => o.TotalAmount), g.Sum(o => o.Details.Sum(d => (int)d.Quantity)), g.Min(o => o.SaleDate), g.Max(o => o.SaleDate), g.Any() ? g.Sum(o => o.TotalAmount) / g.Count() : 0)).OrderByDescending(e => e.TotalSpent).Take(topLimit).ToList();

        return response;
    }

    private async Task<CustomerSummaryResponse> GetSummaryAsync(CancellationToken ct)
    {
        var totalCustomers = await db.BasicCustomers.CountAsync(ct);
        var totalOrders = await db.BasicOrders.CountAsync(ct);
        var totalRevenue = await db.BasicOrders.SumAsync(o => o.TotalAmount, ct);
        var averageOrderValue = totalRevenue / (totalOrders > 0 ? totalOrders : 1);

        var summary = new CustomerSummaryResponse
        {
            TotalCustomers = totalCustomers,
            TotalOrders = totalOrders,
            TotalRevenue = totalRevenue,
            AverageOrderValue = averageOrderValue
        };
        return summary;
    }
}
