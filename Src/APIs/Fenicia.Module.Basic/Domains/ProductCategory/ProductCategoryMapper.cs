using Fenicia.Common.Data.Models.Basic;
using Fenicia.Module.Basic.Domains.ProductCategory.DTOs;
using Riok.Mapperly.Abstractions;

namespace Fenicia.Module.Basic.Domains.ProductCategory;

[Mapper]
public static partial class ProductCategoryMapper
{
    public static GetAllProductCategoryResponse MapToGetAllProductCategoryResponse(this ProductCategoryModel category)
    {
        return new GetAllProductCategoryResponse(category.Id, category.Name);
    }

    public static GetProductCategoryByIdResponse MapToGetProductCategoryByIdResponse(this ProductCategoryModel category)
    {
        return new GetProductCategoryByIdResponse(category.Id, category.Name);
    }

    public static AddProductCategoryResponse MapToAddProductCategoryResponse(this ProductCategoryModel category)
    {
        return new AddProductCategoryResponse(category.Id, category.Name);
    }

    public static UpdateProductCategoryResponse MapToUpdateProductCategoryResponse(this ProductCategoryModel category)
    {
        return new UpdateProductCategoryResponse(category.Id, category.Name);
    }
}
