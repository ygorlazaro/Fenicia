using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.DTOs.Auth.Order;

namespace Fenicia.Auth.Domains.Order;

/// <summary>
/// Mapper class for mapping between OrderModel and OrderResponse/OrderRequest.
/// </summary>
public static class OrderMapper
{
    /// <summary>
    /// Maps an OrderModel to an OrderResponse.
    /// </summary>
    /// <param name="order">The order model.</param>
    /// <returns>The order response.</returns>
    public static OrderResponse MapToOrderResponse(OrderModel order)
    {
        return new OrderResponse(order.Id);
    }

    /// <summary>
    /// Maps an OrderRequest to an OrderModel.
    /// </summary>
    /// <param name="request">The order request.</param>
    /// <returns>The order model.</returns>
    public static OrderModel MapToOrderModel(OrderRequest request)
    {
        return new OrderModel
        {
            Id = request.UserId,
            UserId = request.UserId,
            CompanyId = request.CompanyId,
            OrderNumber = string.Empty,
            TotalAmount = 0,
            DiscountAmount = 0,
            TotalQuantity = 0,
            SaleDate = DateTime.UtcNow,
            Status = Fenicia.Common.Enums.Auth.EnumOrderStatus.Pending,
            PaymentMethod = Fenicia.Common.Enums.Basic.EnumPaymentMethod.CreditCard
        };
    }
}
