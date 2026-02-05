using Fenicia.Common.Data.Models.Basic;
using Fenicia.Common.Data.Requests.Basic;
using Fenicia.Common.Data.Responses.Basic;

namespace Fenicia.Common.Data.Converters.Basic;

public static class ProductConverter
{
    public static ProductResponse Convert(ProductModel model)
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

    public static ProductModel Convert(ProductRequest request)
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

    public static List<ProductResponse> Convert(List<ProductModel> models)
    {
        return models.Select(Convert).ToList();
    }
}
