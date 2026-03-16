using Fenicia.Module.Basic.Domains.Employee.Common;

namespace Fenicia.Module.Basic.Domains.Employee.Commands;

/// <summary>
///     Command record for updating an existing employee.
///     Contains all employee information that can be updated.
/// </summary>
public record UpdateEmployeeCommand(
    Guid Id, 
    Guid PositionId, 
    string Name, 
    string? Email, 
    string? Document, 
    string? PhoneNumber, 
    AddressDTO? Address);