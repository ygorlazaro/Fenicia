namespace Fenicia.Module.Basic.Domains.Order.DTOs.Responses;

public record SalesTrendResponse(string Period, DateTime Date, int OrderCount, decimal TotalValue, int TotalItems);