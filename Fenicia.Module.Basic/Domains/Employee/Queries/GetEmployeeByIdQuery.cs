using Fenicia.Module.Basic.Domains.Employee.Responses;

using MediatR;

namespace Fenicia.Module.Basic.Domains.Employee.Queries;

public record GetEmployeeByIdQuery(Guid Id) : IRequest<GetEmployeeByIdResponse?>;
