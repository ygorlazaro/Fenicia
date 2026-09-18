namespace Fenicia.Common.DTOs.SocialNetwork.Report;

public class GetAllReportQuery()
{
    public int Page { get; set; } = 1;
    public int PerPage { get; set; } = 10;
    public string? Query { get; set; } = null;
    public string? Sort { get; set; } = null;

    public GetAllReportQuery(int Page = 1, int PerPage = 10, string? Query = null, string? Sort = null)
        : this()
    {
        this.Page = Page;
        this.PerPage = PerPage;
        this.Query = Query;
        this.Sort = Sort;
    }
}
