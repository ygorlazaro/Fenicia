using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.Auth.Order;

public class CreateNewOrderResponse()
{
    public CreateNewOrderResponse(Guid orderId)
        : this()
    {
        OrderId = orderId;
    }

    [Required]
    public Guid OrderId { get; set; }
}
