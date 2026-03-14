namespace Fenicia.Module.Basic.Domains.Customer.Commands;

/// <summary>
///     Command record for creating a new customer.
///     Contains all necessary information to create a customer and their associated person record.
/// </summary>
public record AddCustomerCommand(Guid Id, string Name, string? Email, string? Document, string? City, string? Complement, string? Neighborhood, string? Number, Guid? StateId, string? Street, string? ZipCode, string? PhoneNumber);