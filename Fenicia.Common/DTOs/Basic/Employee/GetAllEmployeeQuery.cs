namespace Fenicia.Common.DTOs.Basic.Employee;

public class GetAllEmployeeQuery()
{
    public GetAllEmployeeQuery(int page = 1, int perPage = 10, string? query = null, string? sort = null)
        : this()
    {
        Page = page;
        PerPage = perPage;
        Query = query;
        Sort = sort;
    }

    public int Page { get; set; }

    public int PerPage { get; set; }

    public string? Query { get; set; }

    public string? Sort { get; set; }

    public Dictionary<string, string>? Filters { get; set; } = [];
}
