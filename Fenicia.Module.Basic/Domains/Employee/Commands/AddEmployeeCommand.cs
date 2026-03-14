namespace Fenicia.Module.Basic.Domains.Employee.Commands;

/// <summary>
///     Command record for creating a new employee.
///     Contains all necessary information to create an employee and their associated person record.
/// </summary>
public record AddEmployeeCommand(Guid Id, Guid PositionId, string Name, string? Email, string? Document, string? City, string? Complement, string? Neighborhood, string? Number, Guid StateId, string? Street, string? ZipCode, string? PhoneNumber);