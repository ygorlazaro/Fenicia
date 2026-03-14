namespace Fenicia.Module.Basic.Domains.Employee.Responses;

/// <summary>
/// Response model for retrieving a single employee's detailed information.
/// Contains complete employee and person details.
/// </summary>
public record GetEmployeeByIdResponse(
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
    string? City);
