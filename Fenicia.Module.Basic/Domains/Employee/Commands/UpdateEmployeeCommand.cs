using Fenicia.Module.Basic.Domains.Employee.Common;
using Fenicia.Module.Basic.Domains.Employee.Responses;

using MediatR;

namespace Fenicia.Module.Basic.Domains.Employee.Commands;

public record UpdateEmployeeCommand(
    Guid Id,
    Guid PositionId,
    string Name,
    string? Email,
    string? Document,
    string? PhoneNumber,
    AddressDTO? Address) : IRequest<UpdateEmployeeResponse?>;
