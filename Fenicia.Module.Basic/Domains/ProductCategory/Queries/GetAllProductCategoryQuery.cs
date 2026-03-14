namespace Fenicia.Module.Basic.Domains.ProductCategory.Queries;

/// <summary>
///     Query record for retrieving all product categories with pagination.
/// </summary>
public record GetAllProductCategoryQuery(
    /// <summary>
    /// Page number for pagination.
    /// </summary>
    int Page = 1,
    /// <summary>
    /// Number of items per page.
    /// </summary>
    int PerPage = 10);