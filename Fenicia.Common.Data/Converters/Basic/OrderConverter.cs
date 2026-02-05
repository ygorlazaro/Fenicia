using Fenicia.Common.Data.Models.Basic;
using Fenicia.Common.Data.Requests.Basic;
using Fenicia.Common.Data.Responses.Basic;

namespace Fenicia.Common.Data.Converters.Basic;

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
