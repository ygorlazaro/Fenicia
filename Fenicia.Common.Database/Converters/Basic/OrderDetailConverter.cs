using Fenicia.Common.Database.Models.Basic;
using Fenicia.Common.Database.Requests.Basic;
using Fenicia.Common.Database.Responses.Basic;

namespace Fenicia.Common.Database.Converters.Basic;

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
