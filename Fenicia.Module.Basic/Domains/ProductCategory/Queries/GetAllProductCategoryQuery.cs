using MediatR;
using Fenicia.Module.Basic.Domains.ProductCategory.Responses;
using Fenicia.Common;

namespace Fenicia.Module.Basic.Domains.ProductCategory.Queries;

/// <summary>
///     Query record for retrieving all product categories with pagination.
/// </summary>
public record GetAllProductCategoryQuery(
    /// <summary>
    /// Page number for pagination.
    /// </summary>
    int Page = 1,
    /// <summary>
    /// Number of items per page.
    /// </summary>
    int PerPage = 10) : IRequest<Pagination<List<GetAllProductCategoryResponse>>>;