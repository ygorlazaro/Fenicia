using Fenicia.Common.Data.Models.Basic;
using Fenicia.Common.DTOs.Basic.ProductCategory;
using Riok.Mapperly.Abstractions;

namespace Fenicia.Module.Basic.Domains.ProductCategory;

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class ProductCategoryMapper
{
    public partial GetAllProductCategoryResponse MapToGetAllProductCategoryResponse(ProductCategoryModel category);
    public partial GetProductCategoryByIdResponse MapToGetProductCategoryByIdResponse(ProductCategoryModel category);
    public partial AddProductCategoryResponse MapToAddProductCategoryResponse(ProductCategoryModel category);
    public partial UpdateProductCategoryResponse MapToUpdateProductCategoryResponse(ProductCategoryModel category);
}
