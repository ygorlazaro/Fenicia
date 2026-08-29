namespace Fenicia.Module.Basic.Domains.Employee.DTOs;

public record AddEmployeeCommand(
    Guid Id,
    Guid PositionId,
    string Name,
    string? Email,
    string? Document,
    string? PhoneNumber,
    AddressDTO? Address);
