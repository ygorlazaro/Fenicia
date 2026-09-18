using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.Basic.OrderDetail;

public class GetOrderDetailsByOrderIdQuery()
{
    public GetOrderDetailsByOrderIdQuery(Guid orderId)
        : this()
    {
        OrderId = orderId;
    }

    [Required]
    public Guid OrderId { get; set; }
}
