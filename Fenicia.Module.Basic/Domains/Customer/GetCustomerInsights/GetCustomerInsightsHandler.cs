using Fenicia.Common.Data.Contexts;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Domains.Customer.GetCustomerInsights;

public class GetCustomerInsightsHandler(DefaultContext context)
{
    public async Task<CustomerInsightsResponse> Handle(GetCustomerInsightsQuery query, CancellationToken ct)
    {
        var customers = await context.BasicCustomers
            .Include(c => c.PersonModel)
            .Include(c => c.Orders)
            .ThenInclude(o => o.Details)
            .ToListAsync(ct);

        var orders = await context.BasicOrders
            .Include(o => o.CustomerModel)
            .ThenInclude(c => c.PersonModel)
            .Include(o => o.Details)
            .OrderByDescending(o => o.SaleDate)
            .ToListAsync(ct);

        var now = DateTime.UtcNow;

        // 1. Summary
        var summary = new CustomerSummaryResponse
        {
            TotalCustomers = customers.Count,
            TotalOrders = orders.Count,
            TotalRevenue = orders.Sum(o => o.TotalAmount),
            AverageOrderValue = orders.Count > 0 ? orders.Average(o => o.TotalAmount) : 0,
            AverageCustomerLifetimeValue = customers.Count > 0 
                ? orders.GroupBy(o => o.CustomerId).Average(g => g.Sum(o => o.TotalAmount)) 
                : 0
        };

        // 2. Top Customers by Total Spent
        var topCustomers = orders
            .GroupBy(o => new { o.CustomerId, CustomerName = o.CustomerModel.PersonModel.Name })
            .Select(g => new CustomerOrderHistoryResponse(
                g.Key.CustomerId,
                g.Key.CustomerName,
                g.Count(),
                g.Sum(o => o.TotalAmount),
                g.Sum(o => o.Details.Sum(d => (int)d.Quantity)),
                g.Min(o => o.SaleDate),
                g.Max(o => o.SaleDate),
                g.Count() > 0 ? g.Sum(o => o.TotalAmount) / g.Count() : 0
            ))
            .OrderByDescending(c => c.TotalSpent)
            .Take(query.TopLimit)
            .ToList();

        // 3. Recent Orders
        var recentOrders = orders
            .Take(query.TopLimit)
            .Select(o => new CustomerRecentOrdersResponse(
                o.Id,
                o.CustomerId,
                o.CustomerModel.PersonModel.Name,
                o.TotalAmount,
                o.SaleDate,
                o.Status.ToString(),
                o.Details.Sum(d =>(int)d.Quantity)
            ))
            .ToList();

        // 4. At-Risk Customers (haven't ordered in X days)
        var customerOrderHistory = orders
            .GroupBy(o => o.CustomerId)
            .ToDictionary(
                g => g.Key,
                g => new
                {
                    OrderCount = g.Count(),
                    LastOrderDate = g.Max(o => o.SaleDate),
                    TotalSpent = g.Sum(o => o.TotalAmount)
                });

        var atRiskCustomers = customers
            .Where(c => customerOrderHistory.ContainsKey(c.Id))
            .Select(c =>
            {
                var history = customerOrderHistory[c.Id];
                var daysSinceLastOrder = (now - history.LastOrderDate).Days;
                var riskLevel = daysSinceLastOrder >= query.RiskThresholdDays * 2 ? "High" :
                               daysSinceLastOrder >= query.RiskThresholdDays ? "Medium" : "Low";

                return new CustomerRiskAlertResponse(
                    c.Id,
                    c.PersonModel.Name,
                    history.OrderCount,
                    history.LastOrderDate,
                    daysSinceLastOrder,
                    history.TotalSpent,
                    riskLevel
                );
            })
            .Where(c => c.DaysSinceLastOrder >= query.RiskThresholdDays)
            .OrderByDescending(c => c.DaysSinceLastOrder)
            .Take(query.TopLimit)
            .ToList();

        return new CustomerInsightsResponse
        {
            Summary = summary,
            TopCustomers = topCustomers,
            RecentOrders = recentOrders,
            AtRiskCustomers = atRiskCustomers
        };
    }
}
