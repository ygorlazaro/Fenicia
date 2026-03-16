using Fenicia.Module.Basic.Domains.Customer.Common;

namespace Fenicia.Module.Basic.Domains.Customer.Commands;

/// <summary>
///     Command record for updating an existing customer.
///     Contains all customer information that can be updated.
/// </summary>
public record UpdateCustomerCommand(
    Guid Id, 
    string Name, 
    string? Email, 
    string? Document, 
    string? PhoneNumber, 
    AddressDTO? Address);