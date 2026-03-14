namespace Fenicia.Module.Basic.Domains.Supplier.Responses;

/// <summary>
/// Response record for an updated supplier.
/// </summary>
public record UpdateSupplierResponse(
    /// <summary>
    /// Unique identifier of the supplier.
    /// </summary>
    Guid Id,
    /// <summary>
    /// CNPJ of the supplier.
    /// </summary>
    string? Cnpj);