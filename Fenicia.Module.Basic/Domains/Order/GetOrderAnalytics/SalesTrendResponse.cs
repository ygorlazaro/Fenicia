namespace Fenicia.Module.Basic.Domains.Order.GetOrderAnalytics;

public record SalesTrendResponse(
    string Period,
    DateTime Date,
    int OrderCount,
    decimal TotalValue,
    int TotalItems);
