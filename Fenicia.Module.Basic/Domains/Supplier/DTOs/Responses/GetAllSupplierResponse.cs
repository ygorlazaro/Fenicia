using Fenicia.Module.Basic.Domains.Customer.DTOs.Responses;

namespace Fenicia.Module.Basic.Domains.Supplier.DTOs.Responses;

public record GetAllSupplierResponse(

Guid Id,

Guid PersonId,

string Name,

string? Email,

string? PhoneNumber,

string? Document,

AddressResponse? Address);