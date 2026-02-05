using Fenicia.Common.Enums;

namespace Fenicia.Common.Data.Requests.Basic;

public class StockMovementRequest
{
    public Guid Id
    {
        get;
        set;
    }

    public int Quantity
    {
        get;
        set;
    }

    public DateTime? Date
    {
        get;
        set;
    }

    public decimal Price
    {
        get;
        set;
    }

    public StockMovementType Type
    {
        get;
        set;
    }

    public Guid ProductId
    {
        get;
        set;
    }

    public Guid? CustomerId
    {
        get;
        set;
    }

    public Guid? SupplierId
    {
        get;
        set;
    }
}
