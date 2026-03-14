namespace Fenicia.Module.Basic.Domains.Supplier.Commands;

/// <summary>
/// Command record for deleting a supplier.
/// </summary>
public record DeleteSupplierCommand(
    /// <summary>
    /// Unique identifier of the supplier to delete.
    /// </summary>
    Guid Id);
