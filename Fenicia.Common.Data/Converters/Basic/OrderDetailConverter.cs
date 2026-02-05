using Fenicia.Common.Data.Models.Basic;
using Fenicia.Common.Data.Requests.Basic;
using Fenicia.Common.Data.Responses.Basic;

namespace Fenicia.Common.Data.Converters.Basic;

public static class OrderDetailConverter
{
    public static List<OrderDetailModel> Convert(List<OrderDetailRequest> requests)
    {
        return requests.Select(Convert).ToList();
    }

    public static OrderDetailModel Convert(OrderDetailRequest request)
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

    public static List<OrderDetailResponse> Convert(List<OrderDetailModel> models)
    {
        return models.Select(o => new OrderDetailResponse
        {
            ProductId = o.ProductId,
            Price = o.Price,
            OrderId = o.OrderId,
            Id = o.Id,
            Quantity = o.Quantity
        }).ToList();
    }
}
