namespace Fenicia.Module.Basic.Domains.Employee.Add;

public record AddEmployeeCommand(
    Guid Id,
    Guid PositionId,
    string Name,
    string? Email,
    string? Document,
    string? City,
    string? Complement,
    string? Neighborhood,
    string? Number,
    Guid StateId,
    string? Street,
    string? ZipCode,
    string? PhoneNumber);
