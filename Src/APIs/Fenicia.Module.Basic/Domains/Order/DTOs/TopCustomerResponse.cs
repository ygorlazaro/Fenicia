namespace Fenicia.Module.Basic.Domains.Order.DTOs;

public record TopCustomerResponse(Guid CustomerId, string CustomerName, int OrderCount, decimal TotalSpent, int TotalItems);
