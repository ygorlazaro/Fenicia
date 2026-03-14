namespace Fenicia.Module.Basic.Domains.Order.Responses;

/// <summary>
/// Response containing top customer information by spending.
/// </summary>
public record TopCustomerResponse(
    Guid CustomerId,
    string CustomerName,
    int OrderCount,
    decimal TotalSpent,
    int TotalItems);
