namespace Fenicia.Module.Basic.Domains.Customer.Responses;

/// <summary>
///     Response model for a customer in the list view.
///     Contains basic customer information for pagination display.
/// </summary>
public record GetAllCustomerResponse(
    Guid Id, 
    Guid PersonId, 
    string Name, 
    string? Email, 
    string? PhoneNumber, 
    string? Document,
    AddressResponse? Address);