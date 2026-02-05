using Fenicia.Common.Database.Models.Basic;
using Fenicia.Common.Database.Requests.Basic;
using Fenicia.Common.Database.Responses.Basic;

namespace Fenicia.Common.Database.Converters.Basic;

public static class ProductCategoryConverter
{
    public static ProductCategoryModel Convert(ProductCategoryRequest request)
    {
        return new ProductCategoryModel
        {
            Id = request.Id,
            Name = request.Name
        };
    }

    public static ProductCategoryResponse Convert(ProductCategoryModel model)
    {
        return new ProductCategoryResponse
        {
            Id = model.Id,
            Name = model.Name
        };
    }

    public static List<ProductCategoryResponse> Convert(List<ProductCategoryModel> models)
    {
        return models.Select(Convert).ToList();
    }
}
