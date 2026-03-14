namespace Fenicia.Module.Basic.Domains.Order.Responses;

/// <summary>
/// Response containing order summary for list views.
/// </summary>
public record GetAllOrderResponse(
    Guid Id,
    Guid UserId,
    Guid CustomerId,
    string CustomerName,
    decimal TotalAmount,
    DateTime SaleDate,
    string Status,
    int TotalItems,
    Guid? EmployeeId = null,
    string? EmployeeName = null);
