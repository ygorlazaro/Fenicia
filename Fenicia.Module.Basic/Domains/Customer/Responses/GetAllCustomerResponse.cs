namespace Fenicia.Module.Basic.Domains.Customer.Responses;

public record GetAllCustomerResponse(
    Guid Id,
    Guid PersonId,
    string Name,
    string? Email,
    string? PhoneNumber,
    string? Document,
    AddressResponse? Address);