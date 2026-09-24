namespace Fenicia.Common.DTOs.Basic.Employee;

/// <summary>
/// Query DTO for retrieving employees with pagination and filtering.
/// </summary>
public class EmployeeQuery()
{
    /// <summary>
    /// Initializes a new instance of the <see cref="EmployeeQuery"/> class with the specified parameters.
    /// </summary>
    /// <param name="page">The page number (default: 1).</param>
    /// <param name="perPage">The number of items per page (default: 10).</param>
    /// <param name="query">The search query (optional).</param>
    /// <param name="sort">The sort field (optional).</param>
    public EmployeeQuery(int page = 1, int perPage = 10, string? query = null, string? sort = null)
        : this()
    {
        Page = page;
        PerPage = perPage;
        Query = query;
        Sort = sort;
    }

    /// <summary>
    /// Gets or sets the page number.
    /// </summary>
    public int Page { get; set; }

    /// <summary>
    /// Gets or sets the number of items per page.
    /// </summary>
    public int PerPage { get; set; }

    /// <summary>
    /// Gets or sets the search query.
    /// </summary>
    public string? Query { get; set; }

    /// <summary>
    /// Gets or sets the sort field.
    /// </summary>
    public string? Sort { get; set; }

    /// <summary>
    /// Gets or sets additional filters.
    /// </summary>
    public Dictionary<string, string>? Filters { get; set; } = [];
}