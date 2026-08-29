namespace Fenicia.Module.Basic.Domains.Employee.DTOs;

public record UpdateEmployeeCommand(
    Guid Id,
    Guid PositionId,
    string Name,
    string? Email,
    string? Document,
    string? PhoneNumber,
    AddressDTO? Address);
