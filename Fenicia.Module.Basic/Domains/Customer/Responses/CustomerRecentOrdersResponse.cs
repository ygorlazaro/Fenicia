namespace Fenicia.Module.Basic.Domains.Customer.Responses;

public record CustomerRecentOrdersResponse(Guid OrderId, Guid CustomerId, string CustomerName, decimal TotalAmount, DateTime SaleDate, string Status, int TotalItems);