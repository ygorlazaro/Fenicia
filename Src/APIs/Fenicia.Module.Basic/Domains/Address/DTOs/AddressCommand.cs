namespace Fenicia.Module.Basic.Domains.Address.DTOs;

public record AddressCommand(
    string Street,
    string Number,
    string? Complement,
    string? Neighborhood,
    string ZipCode,
    Guid StateId,
    string City,
    string? Country);
