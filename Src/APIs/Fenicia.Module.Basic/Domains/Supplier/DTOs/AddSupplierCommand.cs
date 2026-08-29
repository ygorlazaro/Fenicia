namespace Fenicia.Module.Basic.Domains.Supplier.DTOs;

public record AddSupplierCommand(

Guid Id,

string Name,

string? Email,

string? Document,

string? PhoneNumber,

string? Cnpj,

AddressDTO? Address);
