namespace Fenicia.Common;

public class Pagination<T>(T data, int total, int page, int perPage)
{
    public T Data
    {
        get; set;
    } = data;

    public int Total
    {
        get; set;
    } = total;

    public int Page
    {
        get; set;
    } = page;

    public int PerPage
    {
        get; set;
    } = perPage;

    public int Pages => (int)Math.Ceiling(Total / (double)PerPage);
}
