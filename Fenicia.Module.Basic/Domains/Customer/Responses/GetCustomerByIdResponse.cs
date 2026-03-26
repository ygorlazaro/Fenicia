namespace Fenicia.Module.Basic.Domains.Customer.Responses;

/// <summary>
///     Response model for retrieving a single customer's detailed information.
///     Contains complete customer and person details.
/// </summary>
public record GetCustomerByIdResponse(
    Guid Id, 
    Guid PersonId, 
    string Name, 
    string? Email, 
    string? PhoneNumber, 
    string? Document,
    AddressResponse? Address);