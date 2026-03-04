namespace Fenicia.Module.Basic.Domains.Employee.GetAll;

public record GetAllEmployeeResponse(
    Guid Id,
    Guid PositionId,
    Guid PersonId,
    string Name,
    string? Email,
    string? PhoneNumber,
    string? Document,
    string? Street,
    string? Number,
    string? Complement,
    string? Neighborhood,
    string? ZipCode,
    Guid? StateId,
    string? City,
    string? PositionName,
    string? StateName);
