namespace Fenicia.Module.Basic.Domains.Supplier.Commands;

/// <summary>
///     Command record for updating an existing supplier.
/// </summary>
public record UpdateSupplierCommand(
    /// <summary>
    /// Unique identifier of the supplier to update.
    /// </summary>
    Guid Id,
    /// <summary>
    /// Updated name of the supplier.
    /// </summary>
    string Name,
    /// <summary>
    /// Updated email address.
    /// </summary>
    string? Email,
    /// <summary>
    /// Updated document number.
    /// </summary>
    string? Document,
    /// <summary>
    /// Updated city name.
    /// </summary>
    string? City,
    /// <summary>
    /// Updated address complement.
    /// </summary>
    string? Complement,
    /// <summary>
    /// Updated neighborhood name.
    /// </summary>
    string? Neighborhood,
    /// <summary>
    /// Updated address number.
    /// </summary>
    string? Number,
    /// <summary>
    /// Updated state ID.
    /// </summary>
    Guid StateId,
    /// <summary>
    /// Updated street address.
    /// </summary>
    string? Street,
    /// <summary>
    /// Updated zip code.
    /// </summary>
    string? ZipCode,
    /// <summary>
    /// Updated phone number.
    /// </summary>
    string? PhoneNumber,
    /// <summary>
    /// Updated CNPJ number.
    /// </summary>
    string? Cnpj);