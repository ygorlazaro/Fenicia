namespace Fenicia.Module.Basic.Domains.Employee;

public record EmployeeResponse(
    Guid Id,
    Guid PositionId,
    string PositionName,
    PersonResponse Person);

public record PersonResponse(
    string Name,
    string? Email,
    string? Cpf,
    string? Phone,
    AddressResponse? Address);

public record AddressResponse(
    string? City,
    string? Complement,
    string? Neighborhood,
    string? Number,
    Guid StateId,
    string? Street,
    string? ZipCode);
