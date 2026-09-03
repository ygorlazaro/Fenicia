using Fenicia.Common;
using Fenicia.Module.Basic.Domains.ProductCategory.DTOs;

namespace Fenicia.Module.Basic.Domains.ProductCategory.Interfaces;

public interface IProductCategoryService
{
    Task<Pagination<List<GetAllProductCategoryResponse>>> GetAllAsync(
        GetAllProductCategoryQuery query,
        CancellationToken cancellationToken = default);

    Task<GetProductCategoryByIdResponse?> GetByIdAsync(
        GetProductCategoryByIdQuery query,
        CancellationToken cancellationToken = default);

    Task<AddProductCategoryResponse> AddAsync(
        AddProductCategoryCommand command,
        Guid companyId,
        CancellationToken cancellationToken = default);

    Task<UpdateProductCategoryResponse?> UpdateAsync(
        UpdateProductCategoryCommand command,
        Guid companyId,
        CancellationToken cancellationToken = default);

    Task<List<GetProductCategoryByIdResponse>> GetByIdsAsync(
        IEnumerable<Guid> ids,
        CancellationToken cancellationToken = default);

    Task DeleteAsync(
        DeleteProductCategoryCommand command,
        Guid companyId,
        CancellationToken cancellationToken = default);
}