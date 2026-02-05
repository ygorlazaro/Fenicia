namespace Fenicia.Common.Database.Requests.Basic;

public class ProductRequest
{
    public Guid Id
    {
        get;
        set;
    }

    public string Name
    {
        get;
        set;
    }

    public decimal CostPrice
    {
        get;
        set;
    }

    public decimal SellingPrice
    {
        get;
        set;
    }

    public int Quantity
    {
        get;
        set;
    }

    public Guid CategoryId
    {
        get;
        set;
    }
}
