namespace Fenicia.Module.Basic.Domains.Order.Responses;

/// <summary>
/// Response containing sales data for a specific period.
/// </summary>
public record SalesTrendResponse(
    string Period,
    DateTime Date,
    int OrderCount,
    decimal TotalValue,
    int TotalItems);
