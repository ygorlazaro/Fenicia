using Fenicia.Common;
using Fenicia.Module.Basic.Domains.Employee.Responses;

using MediatR;

namespace Fenicia.Module.Basic.Domains.Employee.Queries;

/// <summary>
///     Query record for retrieving employees filtered by position ID.
/// </summary>
public record GetEmployeesByPositionIdQuery(Guid PositionId, int Page = 1, int PerPage = 10) : IRequest<Pagination<List<GetEmployeesByPositionIdResponse>>>;
