using Fenicia.Module.Basic.Domains.Employee.Responses;

using MediatR;

namespace Fenicia.Module.Basic.Domains.Employee.Queries;

/// <summary>
///     Query record for retrieving a specific employee by their unique identifier.
/// </summary>
public record GetEmployeeByIdQuery(Guid Id) : IRequest<GetEmployeeByIdResponse?>;
