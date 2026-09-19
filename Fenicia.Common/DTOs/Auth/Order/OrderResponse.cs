using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.Auth.Order;

public class OrderResponse()
{
    public OrderResponse(Guid orderId)
        : this()
    {
        OrderId = orderId;
    }

    [Required]
    public Guid OrderId { get; set; }
}
