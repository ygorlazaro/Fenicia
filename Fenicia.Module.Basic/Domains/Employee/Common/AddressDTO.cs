namespace Fenicia.Module.Basic.Domains.Employee.Common;

public record AddressDTO(
    string Street,
    string Number,
    string? Complement,
    string? Neighborhood,
    string ZipCode,
    Guid StateId,
    string City,
    string? Country);
