using Fenicia.Common.Data.Models.Basic;
using Fenicia.Common.Data.Requests.Basic;
using Fenicia.Common.Data.Responses.Basic;

namespace Fenicia.Common.Data.Mappers.Basic;

public static class ProductMapper
{
    public static ProductResponse Map(ProductModel model)
    {
        return new ProductResponse
        {
            Id = model.Id,
            Name = model.Name,
            CostPrice = model.CostPrice,
            SellingPrice = model.SalesPrice,
            Quantity = model.Quantity,
            CategoryId = model.CategoryId
        };
    }

    public static ProductModel Map(ProductRequest request)
    {
        return new ProductModel
        {
            Id = request.Id,
            Name = request.Name,
            CostPrice = request.CostPrice,
            SalesPrice = request.SellingPrice,
            Quantity = request.Quantity,
            CategoryId = request.CategoryId
        };
    }

    public static List<ProductResponse> Map(List<ProductModel> models)
    {
        return [.. models.Select(Map)];
    }
}
