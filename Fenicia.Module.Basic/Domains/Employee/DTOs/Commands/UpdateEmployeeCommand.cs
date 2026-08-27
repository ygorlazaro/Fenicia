using Fenicia.Module.Basic.Domains.Employee.Common;
using Fenicia.Module.Basic.Domains.Employee.DTOs.Responses;


namespace Fenicia.Module.Basic.Domains.Employee.DTOs.Commands;

public record UpdateEmployeeCommand(
    Guid Id,
    Guid PositionId,
    string Name,
    string? Email,
    string? Document,
    string? PhoneNumber,
    AddressDTO? Address);
