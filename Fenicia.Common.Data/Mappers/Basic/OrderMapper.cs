using Fenicia.Common.Data.Models.Basic;
using Fenicia.Common.Data.Requests.Basic;
using Fenicia.Common.Data.Responses.Basic;

namespace Fenicia.Common.Data.Mappers.Basic;

public static class OrderMapper
{
    public static OrderModel Map(OrderRequest request)
    {
        return new OrderModel
        {
            CustomerId = request.CustomerId,
            Details = OrderDetailConverter.Map(request.Details),
            SaleDate = request.SaleDate,
            Status = request.Status,
            UserId = request.UserId,
            Id = request.Id
        };
    }

    public static OrderResponse Map(OrderModel model)
    {
        return new OrderResponse
        {
            CustomerId = model.CustomerId,
            Details = OrderDetailConverter.Map(model.Details),
            SaleDate = model.SaleDate,
            Status = model.Status,
            TotalAmount = model.TotalAmount,
            UserId = model.UserId,
            Id = model.Id
        };
    }
}
