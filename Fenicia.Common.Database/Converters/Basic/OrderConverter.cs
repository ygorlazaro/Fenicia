using Fenicia.Common.Database.Models.Basic;
using Fenicia.Common.Database.Requests.Basic;
using Fenicia.Common.Database.Responses.Basic;

namespace Fenicia.Common.Database.Converters.Basic;

public static class OrderConverter
{
    public static OrderModel Convert(OrderRequest request)
    {
        return new OrderModel
        {
            CustomerId = request.CustomerId,
            Details = OrderDetailConverter.Convert(request.Details),
            SaleDate = request.SaleDate,
            Status = request.Status,
            TotalAmount = request.TotalAmount,
            UserId = request.UserId,
            Id = request.Id
        };
    }

    public static OrderResponse Convert(OrderModel model)
    {
        return new OrderResponse
        {
            CustomerId = model.CustomerId,
            Details = OrderDetailConverter.Convert(model.Details),
            SaleDate = model.SaleDate,
            Status = model.Status,
            TotalAmount = model.TotalAmount,
            UserId = model.UserId,
            Id = model.Id
        };
    }
}
