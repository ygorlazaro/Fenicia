namespace Fenicia.Module.Basic.Domains.Employee.Responses;

/// <summary>
///     Response model for an employee in the list view.
///     Contains employee information including person and position details.
/// </summary>
public record GetAllEmployeeResponse(Guid Id, Guid PositionId, Guid PersonId, string Name, string? Email, string? PhoneNumber, string? Document, string? Street, string? Number, string? Complement, string? Neighborhood, string? ZipCode, Guid? StateId, string? City, string? PositionName, string? StateName);