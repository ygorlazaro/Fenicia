namespace Fenicia.Common.DTOs.Auth.State;

/// <summary>
/// Request DTO for querying states with pagination and filtering options.
/// </summary>
public class StateRequest()
{
    /// <summary>
    /// Initializes a new instance of the <see cref="StateRequest"/> class with the specified query parameters.
    /// </summary>
    /// <param name="page">The page number (default: 1).</param>
    /// <param name="perPage">The number of items per page (default: 10).</param>
    /// <param name="query">The search query string (optional).</param>
    /// <param name="sort">The sort field and direction (optional).</param>
    public StateRequest(int page = 1, int perPage = 10, string? query = null, string? sort = null)
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
    public int Page { get; set; } = 1;

    /// <summary>
    /// Gets or sets the number of items per page.
    /// </summary>
    public int PerPage { get; set; } = 10;

    /// <summary>
    /// Gets or sets the search query string.
    /// </summary>
    public string? Query { get; set; }

    /// <summary>
    /// Gets or sets the sort field and direction.
    /// </summary>
    public string? Sort { get; set; }
}
