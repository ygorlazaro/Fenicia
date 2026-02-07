namespace Fenicia.Common.Data.Responses.Basic;

public class OrderDetailResponse
{
    public Guid ProductId
    {
        get;
        set;
    }

    public decimal Price
    {
        get;
        set;
    }

    public Guid OrderId
    {
        get;
        set;
    }

    public Guid Id
    {
        get;
        set;
    }

    public double Quantity
    {
        get;
        set;
    }
}