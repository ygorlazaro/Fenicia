namespace Fenicia.Module.Basic.Domains.Order.Responses;

public record OrderStatusCountResponse(
    string Status,
    int Count,
    decimal TotalValue);
