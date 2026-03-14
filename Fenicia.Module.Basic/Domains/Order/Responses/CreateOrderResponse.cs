using Fenicia.Common.Enums.Auth;

namespace Fenicia.Module.Basic.Domains.Order.Responses;

/// <summary>
///     Response containing the created order information.
/// </summary>
public record CreateOrderResponse(Guid Id, Guid UserId, Guid CustomerId, decimal TotalAmount, DateTime SaleDate, OrderStatus Status, Guid? EmployeeId = null);