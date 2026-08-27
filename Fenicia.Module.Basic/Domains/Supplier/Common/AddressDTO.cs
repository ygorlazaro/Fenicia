namespace Fenicia.Module.Basic.Domains.Supplier.Common;

public record AddressDTO(
    string Street,
    string Number,
    string? Complement,
    string? Neighborhood,
    string ZipCode,
    Guid StateId,
    string City,
    string? Country);
