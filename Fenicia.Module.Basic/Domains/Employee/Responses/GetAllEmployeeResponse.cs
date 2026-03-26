using Fenicia.Module.Basic.Domains.Customer.Responses;

namespace Fenicia.Module.Basic.Domains.Employee.Responses;

/// <summary>
///     Response model for an employee in the list view.
///     Contains employee information including person and position details.
/// </summary>
public record GetAllEmployeeResponse(
    Guid Id, 
    Guid PositionId, 
    Guid PersonId, 
    string Name, 
    string? Email, 
    string? PhoneNumber, 
    string? Document, 
    string? PositionName,
    AddressResponse? Address);