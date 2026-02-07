using Fenicia.Common.Data.Models.Basic;
using Fenicia.Common.Data.Requests.Basic;
using Fenicia.Common.Data.Responses.Basic;

namespace Fenicia.Common.Data.Mappers.Basic;

public static class OrderDetailConverter
{
    public static List<OrderDetailModel> Map(List<OrderDetailRequest> requests)
    {
        return [.. requests.Select(Map)];
    }

    public static OrderDetailModel Map(OrderDetailRequest request)
    {
        return new OrderDetailModel
        {
            ProductId = request.ProductId,
            Price = request.Price,
            OrderId = request.OrderId,
            Id = request.Id,
            Quantity = request.Quantity
        };
    }

    public static List<OrderDetailResponse> Map(List<OrderDetailModel> models)
    {
        return
        [
            .. models.Select(o => new OrderDetailResponse
            {
                ProductId = o.ProductId,
                Price = o.Price,
                OrderId = o.OrderId,
                Id = o.Id,
                Quantity = o.Quantity
            })
        ];
    }
}