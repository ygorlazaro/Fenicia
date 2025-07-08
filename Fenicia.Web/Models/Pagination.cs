namespace Fenicia.Web.Models;

public class Pagination<T>
{
    public T? Data
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
    public int Total
    {
        get; set;
    }
    public int TotalPages
    {
        get; set;
    }
}
