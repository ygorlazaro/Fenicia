namespace Fenicia.Module.Basic.Domains.Customer;

public record CustomerResponse(Guid Id, PersonResponse Person);

public record PersonResponse(
    string Name,
    string? Email,
    string? Document,
    string? Phone,
    AddressResponse? Address);

public record AddressResponse(
    string? City,
    string? Complement,
    string? Neighborhood,
    string? Number,
    Guid StateId,
    string? Street,
    string? ZipCode);
