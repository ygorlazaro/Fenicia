namespace Fenicia.Module.Basic.Domains.Order.Queries;

/// <summary>
/// Query to retrieve order analytics.
/// </summary>
public record GetOrderAnalyticsQuery(
    int Days = 90,
    int TopCustomersLimit = 10);
