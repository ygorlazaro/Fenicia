namespace Fenicia.Common;

public class Pagination<T>
{
    public Pagination(T data, int total, int page, int perPage)
    {
        Data = data;
        Total = total;
        Page = page;
        PerPage = perPage;
    }

    public T Data
    {
        get; set;
    }

    public int Total
    {
        get; set;
    }

    public int Page
    {
        get; set;
    }

    public int PerPage
    {
        get; set;
    }

    public int Pages => (int)Math.Ceiling(Total / (double)PerPage);
}
