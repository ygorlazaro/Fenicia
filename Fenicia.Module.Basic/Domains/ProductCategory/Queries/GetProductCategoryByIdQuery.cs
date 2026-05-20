using MediatR;
using Fenicia.Module.Basic.Domains.ProductCategory.Responses;

namespace Fenicia.Module.Basic.Domains.ProductCategory.Queries;

/// <summary>
///     Query record for retrieving a product category by its ID.
/// </summary>
public record GetProductCategoryByIdQuery(
    /// <summary>
    /// Unique identifier of the category.
    /// </summary>
    Guid Id) : IRequest<GetProductCategoryByIdResponse?>;