using Fenicia.Module.Basic.Domains.DataSource.Responses;

using MediatR;

namespace Fenicia.Module.Basic.Domains.DataSource.Queries;

/// <summary>
///     Query to retrieve all product categories for datasource usage.
/// </summary>
public record GetAllProductCategoryForDataSourceQuery : IRequest<List<GetAllProductCategoryForDataSourceResponse>>;
