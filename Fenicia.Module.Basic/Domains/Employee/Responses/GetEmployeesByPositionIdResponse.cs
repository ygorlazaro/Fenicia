using Fenicia.Module.Basic.Domains.Customer.Responses;

namespace Fenicia.Module.Basic.Domains.Employee.Responses;

public record GetEmployeesByPositionIdResponse(
    Guid Id,
    Guid PositionId,
    Guid PersonId,
    string Name,
    string? Email,
    string? PhoneNumber,
    string? Document,
    string? PositionName,
    AddressResponse? Address);