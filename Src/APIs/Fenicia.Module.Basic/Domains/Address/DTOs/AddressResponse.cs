namespace Fenicia.Module.Basic.Domains.Address.DTOs;

public record AddressResponse(
    Guid Id,
    string Street,
    string Number,
    string? Complement,
    string? Neighborhood,
    string ZipCode,
    Guid StateId,
    string? StateName,
    string City,
    string? Country);
