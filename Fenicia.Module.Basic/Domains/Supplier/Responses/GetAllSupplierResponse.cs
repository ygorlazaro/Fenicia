using Fenicia.Module.Basic.Domains.Customer.Responses;

namespace Fenicia.Module.Basic.Domains.Supplier.Responses;

/// <summary>
///     Response record for a supplier in a list.
/// </summary>
public record GetAllSupplierResponse(
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
/// Address (optional).
/// </summary>
AddressResponse? Address);