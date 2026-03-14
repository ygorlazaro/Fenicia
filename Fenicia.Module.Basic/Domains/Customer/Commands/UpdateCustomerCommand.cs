namespace Fenicia.Module.Basic.Domains.Customer.Commands;

/// <summary>
///     Command record for updating an existing customer.
///     Contains all customer information that can be updated.
/// </summary>
public record UpdateCustomerCommand(Guid Id, string Name, string? Email, string? Document, string? City, string? Complement, string? Neighborhood, string? Number, Guid StateId, string? Street, string? ZipCode, string? PhoneNumber);