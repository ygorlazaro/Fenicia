using Fenicia.Common;
using Fenicia.Common.Data.Models.Basic;
using Fenicia.Common.DTOs.Basic.Customer;
using Fenicia.Module.Basic.Domains.Insight.Interfaces;
using Fenicia.Module.Basic.Domains.Order.Interfaces;
using Fenicia.Module.Basic.Domains.Customer.Interfaces;

namespace Fenicia.Module.Basic.Domains.Insight;

/// <summary>
/// Service implementation for managing insights.
/// </summary>
/// <param name="orderService">The order service.</param>
/// <param name="customerRepository">The customer repository.</param>
public sealed class InsightService(
    IOrderService orderService,
    ICustomerRepository customerRepository) : IInsightService
{
    /// <inheritdoc />
    public async Task<CustomerInsightsResponse> GetCustomerInsightsAsync(
        CustomerInsightsRequest request,
        CancellationToken cancellationToken = default)
    {
        var summary = await GetSummaryAsync(cancellationToken);
        var topCustomers = await GetTopCustomersAsync(request.TopLimit, cancellationToken);
        var recentOrders = await GetRecentOrdersAsync(request.TopLimit, cancellationToken);
        var atRiskCustomers = await GetAtRiskCustomersAsync(request, cancellationToken);

        return new CustomerInsightsResponse
        {
            Summary = summary,
            TopCustomers = topCustomers,
            RecentOrders = recentOrders,
            AtRiskCustomers = atRiskCustomers
        };
    }

    /// <summary>
    /// Gets customers at risk of churning based on the specified request.
    /// </summary>
    /// <param name="request">The customer insights request containing risk threshold.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>A list of customer risk alert responses.</returns>
    private async Task<List<CustomerRiskAlertResponse>> GetAtRiskCustomersAsync(
        CustomerInsightsRequest request,
        CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;
        var orders = await orderService.GetAtRiskOrdersAsync(cancellationToken);

        var response = orders.GroupBy(o => o.CustomerId).Select(g =>
            {
                var lastOrder = g.Max(o => o.SaleDate);
                var daysSince = (now - lastOrder).Days;
                var riskLevel = daysSince >= request.RiskThresholdDays * 2 ? "High" :
                    daysSince >= request.RiskThresholdDays ? "Medium" : "Low";

                return new CustomerRiskAlertResponse(
                    g.Key,
                    g.First().CustomerName,
                    g.Count(),
                    lastOrder,
                    daysSince,
                    g.Sum(o => o.TotalAmount),
                    riskLevel);
            }).Where(c => c.DaysSinceLastOrder >= request.RiskThresholdDays).OrderByDescending(c => c.DaysSinceLastOrder)
            .ToList();

        return response;
    }

    /// <summary>
    /// Gets recent orders up to the specified limit.
    /// </summary>
    /// <param name="topLimit">The maximum number of orders to return.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>A list of customer recent orders responses.</returns>
    private async Task<List<CustomerRecentOrdersResponse>> GetRecentOrdersAsync(
        int topLimit,
        CancellationToken cancellationToken = default)
    {
        var orders = await orderService.GetRecentOrdersAsync(topLimit, cancellationToken);

        var response = orders.Take(topLimit).Select(o => new CustomerRecentOrdersResponse(
            o.Id,
            o.CustomerId,
            o.CustomerName,
            o.TotalAmount,
            o.SaleDate,
            o.Status.ToString(),
            o.TotalItems)).ToList();

        return response;
    }

    /// <summary>
    /// Gets top customers by order history up to the specified limit.
    /// </summary>
    /// <param name="topLimit">The maximum number of customers to return.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>A list of customer order history responses.</returns>
    private async Task<List<CustomerOrderHistoryResponse>> GetTopCustomersAsync(
        int topLimit,
        CancellationToken cancellationToken = default)
    {
        var orders = await orderService.GetTopCustomerOrdersAsync(cancellationToken);

        var response = orders.GroupBy(o => new { o.CustomerId, CustomerName = o.CustomerName }).Select(g =>
                new CustomerOrderHistoryResponse(
                    g.Key.CustomerId,
                    g.Key.CustomerName,
                    g.Count(),
                    g.Sum(o => o.TotalAmount),
                    g.Sum(o => o.TotalItems),
                    g.Min(o => o.SaleDate),
                    g.Max(o => o.SaleDate),
                    g.Any() ? g.Sum(o => o.TotalAmount) / g.Count() : 0)).OrderByDescending(e => e.TotalSpent)
            .Take(topLimit).ToList();

        return response;
    }

    /// <summary>
    /// Gets the customer summary metrics.
    /// </summary>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>The customer summary response.</returns>
    private async Task<CustomerSummaryResponse> GetSummaryAsync(CancellationToken cancellationToken = default)
    {
        var totalCustomers = await customerRepository.CountAsync(cancellationToken);
        var totalOrders = await orderService.GetTotalOrdersCountAsync(cancellationToken);
        var totalRevenue = await orderService.GetTotalRevenueAsync(cancellationToken);
        var averageOrderValue = totalOrders > 0 ? totalRevenue / totalOrders : 0;

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