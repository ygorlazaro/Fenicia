using Fenicia.Module.Basic.Domains.Customer.Responses;

namespace Fenicia.Module.Basic.Domains.Employee.Responses;

public record GetEmployeeByIdResponse(Guid Id, Guid PositionId, Guid PersonId, string Name, string? Email, string? PhoneNumber, string? Document, AddressResponse? Address);