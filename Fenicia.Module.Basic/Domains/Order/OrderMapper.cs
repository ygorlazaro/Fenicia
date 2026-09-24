using Fenicia.Common.DTOs.Basic.Order;
using Fenicia.Common.Data.Models.Basic;

namespace Fenicia.Module.Basic.Domains.Order;

/// <summary>
/// Mapper class for mapping order data.
/// </summary>
public static class OrderMapper
{
    /// <summary>
    /// Maps an OrderModel to an OrderResponse.
    /// </summary>
    /// <param name="order">The order model.</param>
    /// <param name="totalItems">The total number of items in the order.</param>
    /// <returns>The order response.</returns>
    public static OrderResponse MapToResponse(OrderModel order, int totalItems = 0)
    {
        return new OrderResponse(
            order.Id,
            order.OrderNumber,
            order.UserId,
            order.CustomerId,
            order.Customer.Person.Name,
            order.TotalAmount,
            order.DiscountAmount,
            order.TotalQuantity,
            order.SaleDate,
            order.Status.ToString(),
            order.PaymentMethod,
            order.Notes,
            order.EmployeeId,
            order.Employee?.Person.Name,
            totalItems);
    }

    /// <summary>
    /// Maps an OrderModel to an OrderResponse (for list views).
    /// </summary>
    /// <param name="order">The order model.</param>
    /// <param name="detailCount">The number of detail items.</param>
    /// <returns>The order response.</returns>
    public static OrderResponse MapToListResponse(OrderModel order, int detailCount)
    {
        return new OrderResponse(
            order.Id,
            order.OrderNumber,
            order.UserId,
            order.CustomerId,
            order.Customer.Person.Name,
            order.TotalAmount,
            order.DiscountAmount,
            order.TotalQuantity,
            order.SaleDate,
            order.Status.ToString(),
            order.PaymentMethod,
            order.Notes,
            order.EmployeeId,
            order.Employee?.Person.Name,
            detailCount);
    }
}