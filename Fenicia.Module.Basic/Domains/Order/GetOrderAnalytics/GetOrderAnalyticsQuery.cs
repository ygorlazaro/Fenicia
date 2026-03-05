namespace Fenicia.Module.Basic.Domains.Order.GetOrderAnalytics;

public record GetOrderAnalyticsQuery(
    int Days = 90,
    int TopCustomersLimit = 10);
