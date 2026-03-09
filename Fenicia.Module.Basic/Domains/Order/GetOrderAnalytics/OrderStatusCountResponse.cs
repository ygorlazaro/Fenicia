namespace Fenicia.Module.Basic.Domains.Order.GetOrderAnalytics;

public record OrderStatusCountResponse(
    string Status,
    int Count,
    decimal TotalValue);
