namespace Fenicia.Module.Basic.Domains.Order.Responses;

/// <summary>
/// Response containing order count and total value grouped by status.
/// </summary>
public record OrderStatusCountResponse(
    string Status,
    int Count,
    decimal TotalValue);
