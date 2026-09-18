namespace Fenicia.Common.DTOs.Basic.Product;

public class GetAllProductQuery()
{
    public GetAllProductQuery(int page = 1, int perPage = 10, string? query = null, string? sort = null)
        : this()
    {
        Page = page;
        PerPage = perPage;
        Query = query;
        Sort = sort;
    }

    public int Page { get; set; } = 1;

    public int PerPage { get; set; } = 10;

    public string? Query { get; set; }

    public string? Sort { get; set; }

    public Dictionary<string, string>? Filters { get; init; } = [];
}
