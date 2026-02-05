using Fenicia.Common.Database.Models.Basic;
using Fenicia.Common.Database.Requests.Basic;
using Fenicia.Common.Database.Responses.Basic;

namespace Fenicia.Common.Database.Converters.Basic;

public static class ProductConverter
{
    public static ProductResponse Convert(ProductModel model)
    {
        return new ProductResponse
        {
            Id = model.Id,
            Name = model.Name,
            CostPrice = model.CostPrice,
            SellingPrice = model.SellingPrice,
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
            SellingPrice = request.SellingPrice,
            Quantity = request.Quantity,
            CategoryId = request.CategoryId
        };
    }

    public static List<ProductResponse> Convert(List<ProductModel> models)
    {
        return models.Select(Convert).ToList();
    }
}
