using Fenicia.Auth.Domains.Order.DTOs;
using Fenicia.Common.Data.Models.Auth;

namespace Fenicia.Auth.Domains.Order;

public static class OrderMapper
{
    public static CreateNewOrderResponse MapToCreateNewOrderResponse(this OrderModel order)
    {
        return new CreateNewOrderResponse(order.Id);
    }
}