namespace Fenicia.Module.Basic.Domains.Customer.Common;

/// <summary>
///     Data transfer object for address information.
/// </summary>
public record AddressDTO(
    string Street,
    string Number,
    string? Complement,
    string? Neighborhood,
    string ZipCode,
    Guid StateId,
    string City,
    string? Country);
