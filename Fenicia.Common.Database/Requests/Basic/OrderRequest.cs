using Fenicia.Common.Enums;

namespace Fenicia.Common.Database.Requests.Basic;

public class OrderRequest
{
    public Guid UserId
    {
        get;
        set;
    }

    public Guid CustomerId
    {
        get;
        set;
    }

    public List<OrderDetailRequest> Details
    {
        get;
        set;
    }

    public DateTime SaleDate
    {
        get;
        set;
    }

    public OrderStatus Status
    {
        get;
        set;
    }

    public decimal TotalAmount
    {
        get;
        set;
    }

    public Guid Id
    {
        get;
        set;
    }
}
