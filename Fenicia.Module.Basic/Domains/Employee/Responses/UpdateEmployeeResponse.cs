namespace Fenicia.Module.Basic.Domains.Employee.Responses;

/// <summary>
///     Response model returned after successfully updating an employee.
/// </summary>
public record UpdateEmployeeResponse(Guid Id, Guid PositionId, Guid PersonId);