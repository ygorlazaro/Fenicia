using Fenicia.Common.Data.Models.Basic;
using Fenicia.Common.DTOs.Basic.Product;
using Riok.Mapperly.Abstractions;

namespace Fenicia.Module.Basic.Domains.Product;

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class ProductMapper
{
    [MapProperty("Category.Name", nameof(GetAllProductResponse.CategoryName))]
    [MapProperty("Supplier.Person.Name", nameof(GetAllProductResponse.SupplierName))]
    public partial GetAllProductResponse MapToGetAllProductResponse(ProductModel product);

    [MapProperty("Category.Name", nameof(GetProductByIdResponse.CategoryName))]
    [MapProperty("Supplier.Person.Name", nameof(GetProductByIdResponse.SupplierName))]
    public partial GetProductByIdResponse MapToGetProductByIdResponse(ProductModel product);

    [MapProperty("Category.Name", nameof(GetProductsByCategoryIdResponse.CategoryName))]
    public partial GetProductsByCategoryIdResponse MapToGetProductsByCategoryIdResponse(ProductModel product);

    [MapProperty("Category.Name", nameof(AddProductResponse.CategoryName))]
    [MapProperty("Supplier.Person.Name", nameof(AddProductResponse.SupplierName))]
    public partial AddProductResponse MapToAddProductResponse(ProductModel product);

    [MapProperty("Category.Name", nameof(UpdateProductResponse.CategoryName))]
    [MapProperty("Supplier.Person.Name", nameof(UpdateProductResponse.SupplierName))]
    public partial UpdateProductResponse MapToUpdateProductResponse(ProductModel product);
}
