using Fenicia.Common;
using Fenicia.Common.DTOs.Basic.ProductCategory;

namespace Fenicia.Module.Basic.Domains.ProductCategory.Interfaces;

public interface IProductCategoryService
{
    Task<Pagination<List<GetAllProductCategoryResponse>>> GetAllAsync(
        int page = 1,
        int perPage = 10,
        CancellationToken cancellationToken = default);

    Task<GetProductCategoryByIdResponse?> GetByIdAsync(
        GetProductCategoryByIdQuery query,
        CancellationToken cancellationToken = default);

    Task<AddProductCategoryResponse> AddAsync(
        AddProductCategoryRequest command,
        Guid companyId,
        CancellationToken cancellationToken = default);

    Task<UpdateProductCategoryResponse?> UpdateAsync(
        UpdateProductCategoryRequest command,
        Guid companyId,
        CancellationToken cancellationToken = default);

    Task<List<GetProductCategoryByIdResponse>> GetByIdsAsync(
        IEnumerable<Guid> ids,
        CancellationToken cancellationToken = default);

    Task DeleteAsync(
        DeleteProductCategoryRequest command,
        Guid companyId,
        CancellationToken cancellationToken = default);
}
