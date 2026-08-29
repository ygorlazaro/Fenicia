using Fenicia.Module.Basic.Domains.Address.DTOs;

namespace Fenicia.Module.Basic.Domains.Employee.DTOs;

public record GetEmployeeByIdResponse(Guid Id, Guid PositionId, Guid PersonId, string Name, string? Email, string? PhoneNumber, string? Document, AddressResponse? Address);
