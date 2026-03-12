namespace Fenicia.Module.Basic.Domains.Order.Responses;

public record TopCustomerResponse(
    Guid CustomerId,
    string CustomerName,
    int OrderCount,
    decimal TotalSpent,
    int TotalItems);
