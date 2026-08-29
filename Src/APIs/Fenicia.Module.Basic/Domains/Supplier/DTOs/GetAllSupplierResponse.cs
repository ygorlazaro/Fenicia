using Fenicia.Module.Basic.Domains.Address.DTOs;

namespace Fenicia.Module.Basic.Domains.Supplier.DTOs;

public record GetAllSupplierResponse(

Guid Id,

Guid PersonId,

string Name,

string? Email,

string? PhoneNumber,

string? Document,

AddressResponse? Address);
