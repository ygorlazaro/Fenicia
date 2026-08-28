namespace Fenicia.Module.Basic.Domains.Order.DTOs;

public record OrderStatusCountResponse(string Status, int Count, decimal TotalValue);
