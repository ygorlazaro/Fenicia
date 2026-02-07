using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.Data.Responses.Auth;

namespace Fenicia.Common.Data.Mappers.Auth;

public static class OrderMapper
{
    public static OrderResponse Map(OrderModel order)
    {
        return new OrderResponse
        {
            Id = order.Id,
            SaleDate = order.SaleDate,
            Status = order.Status,
            TotalAmount = order.TotalAmount
        };
    }
}