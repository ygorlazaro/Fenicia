namespace Fenicia.Module.Basic.Domains.Supplier.Commands;

/// <summary>
///     Command record for creating a new supplier.
/// </summary>
public record AddSupplierCommand(
    /// <summary>
    /// Unique identifier for the new supplier.
    /// </summary>
    Guid Id,
    /// <summary>
    /// Name of the supplier.
    /// </summary>
    string Name,
    /// <summary>
    /// Email address (optional).
    /// </summary>
    string? Email,
    /// <summary>
    /// Document number (CPF/CNPJ).
    /// </summary>
    string? Document,
    /// <summary>
    /// City name.
    /// </summary>
    string? City,
    /// <summary>
    /// Address complement (optional).
    /// </summary>
    string? Complement,
    /// <summary>
    /// Neighborhood name.
    /// </summary>
    string? Neighborhood,
    /// <summary>
    /// Address number.
    /// </summary>
    string? Number,
    /// <summary>
    /// State ID.
    /// </summary>
    Guid StateId,
    /// <summary>
    /// Street address.
    /// </summary>
    string? Street,
    /// <summary>
    /// Zip code.
    /// </summary>
    string? ZipCode,
    /// <summary>
    /// Phone number.
    /// </summary>
    string? PhoneNumber,
    /// <summary>
    /// CNPJ number.
    /// </summary>
    string? Cnpj);