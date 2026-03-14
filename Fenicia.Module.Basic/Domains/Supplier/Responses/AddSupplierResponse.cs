namespace Fenicia.Module.Basic.Domains.Supplier.Responses;

/// <summary>
/// Response record for a newly created supplier.
/// </summary>
public record AddSupplierResponse(
    /// <summary>
    /// Unique identifier of the supplier.
    /// </summary>
    Guid Id,
    /// <summary>
    /// CNPJ of the supplier.
    /// </summary>
    string? Cnpj);