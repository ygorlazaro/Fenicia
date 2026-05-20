using Fenicia.Module.Basic.Domains.DataSource.Responses;

using MediatR;

namespace Fenicia.Module.Basic.Domains.DataSource.Queries;

/// <summary>
///     Query to retrieve all positions for datasource usage.
/// </summary>
public record GetAllPositionForDataSourceQuery : IRequest<List<GetAllPositionForDataSourceResponse>>;
