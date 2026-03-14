namespace Fenicia.Module.Basic.Domains.Supplier.Queries;

/// <summary>
///     Query record for retrieving all suppliers with pagination.
/// </summary>
public record GetAllSupplierQuery(
    /// <summary>
    /// Page number for pagination.
    /// </summary>
    int Page = 1,
    /// <summary>
    /// Number of items per page.
    /// </summary>
    int PerPage = 10);