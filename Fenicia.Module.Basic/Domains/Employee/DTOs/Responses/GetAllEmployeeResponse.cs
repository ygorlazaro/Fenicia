using Fenicia.Module.Basic.Domains.Customer.DTOs.Responses;

namespace Fenicia.Module.Basic.Domains.Employee.DTOs.Responses;

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
