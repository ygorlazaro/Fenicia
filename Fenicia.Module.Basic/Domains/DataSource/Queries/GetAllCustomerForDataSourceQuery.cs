using Fenicia.Module.Basic.Domains.DataSource.Responses;

using MediatR;

namespace Fenicia.Module.Basic.Domains.DataSource.Queries;

/// <summary>
///     Query to retrieve all customers for datasource usage.
/// </summary>
public record GetAllCustomerForDataSourceQuery : IRequest<List<GetAllCustomerForDataSourceResponse>>;
