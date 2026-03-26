using Fenicia.Module.Basic.Domains.Employee.Common;

namespace Fenicia.Module.Basic.Domains.Employee.Commands;

/// <summary>
///     Command record for creating a new employee.
///     Contains all necessary information to create an employee and their associated person record.
/// </summary>
public record AddEmployeeCommand(
    Guid Id, 
    Guid PositionId, 
    string Name, 
    string? Email, 
    string? Document, 
    string? PhoneNumber, 
    AddressDTO? Address);