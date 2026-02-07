namespace Fenicia.Common;

public class Pagination<T>(T data, int total, int page, int perPage)
{
    public Pagination(T data, int total, PaginationQuery query)
        : this(data, total, query.Page, query.PerPage)
    {
    }

    public T Data { get; set; } = data;

    public int Total { get; set; } = total;

    public int Page { get; set; } = page;

    public int PerPage { get; set; } = perPage;

    public int Pages => (int)Math.Ceiling(this.Total / (double)this.PerPage);
}