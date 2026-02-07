using Fenicia.Common.Data.Models.Basic;
using Fenicia.Common.Data.Requests.Basic;
using Fenicia.Common.Data.Responses.Basic;

namespace Fenicia.Common.Data.Mappers.Basic;

public static class ProductCategoryMapper
{
    public static ProductCategoryModel Map(ProductCategoryRequest request)
    {
        return new ProductCategoryModel
        {
            Id = request.Id,
            Name = request.Name
        };
    }

    public static ProductCategoryResponse Map(ProductCategoryModel model)
    {
        return new ProductCategoryResponse
        {
            Id = model.Id,
            Name = model.Name
        };
    }

    public static List<ProductCategoryResponse> Map(List<ProductCategoryModel> models)
    {
        return [.. models.Select(Map)];
    }
}