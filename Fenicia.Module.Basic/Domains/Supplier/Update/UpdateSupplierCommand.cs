namespace Fenicia.Module.Basic.Domains.Supplier.Update;

public record UpdateSupplierCommand(
    Guid Id,
    string Name,
    string? Email,
    string? Document,
    string? City,
    string? Complement,
    string? Neighborhood,
    string? Number,
    Guid StateId,
    string? Street,
    string? ZipCode,
    string? PhoneNumber,
    string? Cnpj);