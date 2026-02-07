using Fenicia.Common.Data.Models.Basic;
using Fenicia.Common.Data.Requests.Basic;
using Fenicia.Common.Data.Responses.Auth;

namespace Fenicia.Common.Data.Mappers.Basic;

public static class StockMovementMapper
{
    public static List<StockMovementResponse> Map(List<StockMovementModel> models)
    {
        return [.. models.Select(Map)];
    }

    public static StockMovementModel Map(StockMovementRequest request)
    {
        return new StockMovementModel
        {
            Id = request.Id,
            Quantity = request.Quantity,
            Date = request.Date,
            Price = request.Price,
            Type = request.Type,
            ProductId = request.ProductId,
            CustomerId = request.CustomerId,
            SupplierId = request.SupplierId
        };
    }

    public static StockMovementResponse Map(StockMovementModel model)
    {
        return new StockMovementResponse
        {
            Id = model.Id,
            Quantity = model.Quantity,
            Date = model.Date,
            Price = model.Price,
            Type = model.Type,
            ProductId = model.ProductId,
            CustomerId = model.CustomerId,
            SupplierId = model.SupplierId
        };
    }
}