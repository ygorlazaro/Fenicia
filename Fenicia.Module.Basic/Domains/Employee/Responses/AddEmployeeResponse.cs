namespace Fenicia.Module.Basic.Domains.Employee.Responses;

/// <summary>
/// Response model returned after successfully creating a new employee.
/// </summary>
public record AddEmployeeResponse(
    Guid Id,
    Guid PositionId,
    Guid PersonId);
