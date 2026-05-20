using Fenicia.Module.Basic.Domains.DataSource.Responses;

using MediatR;

namespace Fenicia.Module.Basic.Domains.DataSource.Queries;

/// <summary>
///     Query to retrieve all suppliers for datasource usage.
/// </summary>
public record GetAllSupplierForDataSourceQuery : IRequest<List<GetAllSupplierForDataSourceResponse>>;
