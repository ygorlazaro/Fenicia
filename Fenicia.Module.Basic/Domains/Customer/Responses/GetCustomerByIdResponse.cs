namespace Fenicia.Module.Basic.Domains.Customer.Responses;

/// <summary>
/// Response model for retrieving a single customer's detailed information.
/// Contains complete customer and person details.
/// </summary>
public record GetCustomerByIdResponse(
    Guid Id,
    Guid PersonId,
    string Name,
    string? Email,
    string? PhoneNumber,
    string? Document,
    string? Street,
    string? Number,
    string? Complement,
    string? Neighborhood,
    string? ZipCode,
    Guid? StateId,
    string? City);
