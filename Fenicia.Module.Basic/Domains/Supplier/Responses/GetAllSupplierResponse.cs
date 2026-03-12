namespace Fenicia.Module.Basic.Domains.Supplier.Responses;

public record GetAllSupplierResponse(
    Guid Id,
    Guid PersonId,
    string Name,
    string? Email,
    string? PhoneNumber,
    string? Document,
    string? Street,
    string? Number,
    string? Complement,
    string? Neighborhood,
    string? ZipCode,
    Guid? StateId,
    string? City);
