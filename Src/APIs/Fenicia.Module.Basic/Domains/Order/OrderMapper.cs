using Fenicia.Common.Data.Models.Basic;
using Fenicia.Module.Basic.Domains.Order.DTOs;
using Riok.Mapperly.Abstractions;

namespace Fenicia.Module.Basic.Domains.Order;

[Mapper]
public static partial class OrderMapper
{
    public static GetAllOrderResponse MapToGetAllOrderResponse(this OrderModel order, string customerName, string? employeeName, int detailCount)
    {
        return new GetAllOrderResponse(
            order.Id,
            order.OrderNumber,
            order.UserId,
            order.CustomerId,
            customerName,
            order.TotalAmount,
            order.DiscountAmount,
            order.TotalQuantity,
            order.SaleDate,
            order.Status.ToString(),
            order.PaymentMethod,
            detailCount,
            order.EmployeeId,
            employeeName);
    }

    public static GetOrderByIdResponse MapToGetOrderByIdResponse(this OrderModel order)
    {
        return new GetOrderByIdResponse(
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
            order.EmployeeId);
    }

    public static CreateOrderResponse MapToCreateOrderResponse(this OrderModel order)
    {
        return new CreateOrderResponse(
            order.Id,
            order.OrderNumber,
            order.UserId,
            order.CustomerId,
            order.TotalAmount,
            order.DiscountAmount,
            order.TotalQuantity,
            order.SaleDate,
            order.Status,
            order.PaymentMethod,
            order.Notes,
            order.EmployeeId);
    }
}
