namespace Fenicia.Module.Basic.Domains.Supplier.Responses;

/// <summary>
/// Response record for retrieving a single supplier by ID.
/// </summary>
public record GetSupplierByIdResponse(
    /// <summary>
    /// Unique identifier of the supplier.
    /// </summary>
    Guid Id,
    /// <summary>
    /// Person ID associated with the supplier.
    /// </summary>
    Guid PersonId,
    /// <summary>
    /// Name of the supplier.
    /// </summary>
    string Name,
    /// <summary>
    /// Email address (optional).
    /// </summary>
    string? Email,
    /// <summary>
    /// Phone number.
    /// </summary>
    string? PhoneNumber,
    /// <summary>
    /// Document number.
    /// </summary>
    string? Document,
    /// <summary>
    /// Street address.
    /// </summary>
    string? Street,
    /// <summary>
    /// Address number.
    /// </summary>
    string? Number,
    /// <summary>
    /// Address complement.
    /// </summary>
    string? Complement,
    /// <summary>
    /// Neighborhood name.
    /// </summary>
    string? Neighborhood,
    /// <summary>
    /// Zip code.
    /// </summary>
    string? ZipCode,
    /// <summary>
    /// State ID.
    /// </summary>
    Guid? StateId,
    /// <summary>
    /// City name.
    /// </summary>
    string? City);
