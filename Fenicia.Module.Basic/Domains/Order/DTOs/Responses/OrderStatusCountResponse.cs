namespace Fenicia.Module.Basic.Domains.Order.DTOs.Responses;

public record OrderStatusCountResponse(string Status, int Count, decimal TotalValue);