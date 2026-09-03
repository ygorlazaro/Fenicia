namespace Fenicia.Common;

public class Pagination<T>(T data, int total, int page, int perPage)
{
    public Pagination(T data, int total, PaginationQuery query)
        : this(data, total, 0, 0)
    {
        ArgumentNullException.ThrowIfNull(query);
        Page = query.Page;
        PerPage = query.PerPage;
    }

    public T Data { get; init; } = data;

    public int Total { get; init; } = total;

    public int Page { get; init; } = page;

    public int PerPage { get; init; } = perPage;

    public int Pages => (int)Math.Ceiling(Total / (double)PerPage);
}