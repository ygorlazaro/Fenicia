using Fenicia.Module.Basic.Domains.DataSource.Responses;

using MediatR;

namespace Fenicia.Module.Basic.Domains.DataSource.Queries;

/// <summary>
///     Query to retrieve all employees for datasource usage.
/// </summary>
public record GetAllEmployeeForDataSourceQuery : IRequest<List<GetAllEmployeeForDataSourceResponse>>;
