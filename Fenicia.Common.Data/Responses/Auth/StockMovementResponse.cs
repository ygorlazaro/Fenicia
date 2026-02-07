using Fenicia.Common.Data.Responses.Basic;
using Fenicia.Common.Enums.Basic;

namespace Fenicia.Common.Data.Responses.Auth;

public class StockMovementResponse
{
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

    public DateTime? Date
    {
        get;
        set;
    }

    public decimal? Price
    {
        get;
        set;
    }

    public StockMovementType Type
    {
        get;
        set;
    }

    public ProductResponse Product
    {
        get;
        set;
    }

    public Guid? CustomerId
    {
        get;
        set;
    }

    public Guid? SupplerId
    {
        get;
        set;
    }

    public Guid ProductId
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