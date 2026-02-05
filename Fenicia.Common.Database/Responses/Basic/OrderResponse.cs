using Fenicia.Common.Enums;

namespace Fenicia.Common.Database.Responses.Basic;

public class OrderResponse
{
    public Guid CustomerId
    {
        get;
        set;
    }

    public List<OrderDetailResponse> Details
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

    public Guid UserId
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
