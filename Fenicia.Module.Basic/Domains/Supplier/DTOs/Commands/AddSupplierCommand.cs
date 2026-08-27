using Fenicia.Module.Basic.Domains.Supplier.Common;
using Fenicia.Module.Basic.Domains.Supplier.DTOs.Responses;

namespace Fenicia.Module.Basic.Domains.Supplier.DTOs.Commands;

public record AddSupplierCommand(

Guid Id,

string Name,

string? Email,

string? Document,

string? PhoneNumber,

string? Cnpj,

AddressDTO? Address);