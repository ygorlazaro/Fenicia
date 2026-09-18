using Fenicia.Common.Data.Models.Basic;
using Fenicia.Common.DTOs.Basic.Order;
using Fenicia.Common.Enums.Auth;
using Fenicia.Common.Enums.Basic;
using Riok.Mapperly.Abstractions;

namespace Fenicia.Module.Basic.Domains.Order;

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class OrderMapper
{
    public GetAllOrderResponse MapToGetAllOrderResponse(
        Guid id,
        string orderNumber,
        Guid userId,
        Guid customerId,
        string customerName,
        decimal totalAmount,
        decimal discountAmount,
        int totalQuantity,
        DateTime saleDate,
        OrderStatus status,
        PaymentMethod paymentMethod,
        int detailCount,
        Guid? employeeId,
        string? employeeName)
    {
        return new GetAllOrderResponse(
            id,
            orderNumber,
            userId,
            customerId,
            customerName,
            totalAmount,
            discountAmount,
            totalQuantity,
            saleDate,
            status.ToString(),
            paymentMethod,
            detailCount,
            employeeId,
            employeeName);
    }

    public static GetOrderByIdResponse MapToGetOrderByIdResponse(OrderModel order)
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
            order.EmployeeId,
            order.Employee?.Person.Name);
    }

    public static CreateOrderResponse MapToCreateOrderResponse(OrderModel order)
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
