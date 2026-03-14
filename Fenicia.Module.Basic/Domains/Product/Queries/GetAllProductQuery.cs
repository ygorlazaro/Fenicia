namespace Fenicia.Module.Basic.Domains.Product.Queries;

/// <summary>
///     Query record for retrieving all products with pagination.
/// </summary>
public record GetAllProductQuery(
    /// <summary>
    /// Page number for pagination.
    /// </summary>
    int Page = 1,
    /// <summary>
    /// Number of items per page.
    /// </summary>
    int PerPage = 10);