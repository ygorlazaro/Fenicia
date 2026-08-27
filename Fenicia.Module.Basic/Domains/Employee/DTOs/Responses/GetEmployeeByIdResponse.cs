using Fenicia.Module.Basic.Domains.Customer.DTOs.Responses;

namespace Fenicia.Module.Basic.Domains.Employee.DTOs.Responses;

public record GetEmployeeByIdResponse(Guid Id, Guid PositionId, Guid PersonId, string Name, string? Email, string? PhoneNumber, string? Document, AddressResponse? Address);