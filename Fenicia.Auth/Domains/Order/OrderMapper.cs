using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.DTOs.Auth.Order;

namespace Fenicia.Auth.Domains.Order;

public static class OrderMapper
{
    public static CreateNewOrderResponse MapToCreateNewOrderResponse(this OrderModel order)
    {
        return new CreateNewOrderResponse(order.Id);
    }
}