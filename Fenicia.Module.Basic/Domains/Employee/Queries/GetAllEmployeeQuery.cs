using Fenicia.Common;
using Fenicia.Module.Basic.Domains.Employee.Responses;

using MediatR;

namespace Fenicia.Module.Basic.Domains.Employee.Queries;

/// <summary>
///     Query record for retrieving all employees with pagination.
/// </summary>
public record GetAllEmployeeQuery(int Page = 1, int PerPage = 10) : IRequest<Pagination<List<GetAllEmployeeResponse>>>;
