using Fenicia.Module.Basic.Domains.Customer.Responses;

namespace Fenicia.Module.Basic.Domains.Supplier.Responses;

public record GetAllSupplierResponse(

Guid Id,

Guid PersonId,

string Name,

string? Email,

string? PhoneNumber,

string? Document,

AddressResponse? Address);