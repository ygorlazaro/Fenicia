namespace Fenicia.Module.Basic.Domains.Customer.DTOs;

public record GetAllCustomerResponse(
    Guid Id,
    Guid PersonId,
    string Name,
    string? Email,
    string? PhoneNumber,
    string? Document,
    AddressResponse? Address);
