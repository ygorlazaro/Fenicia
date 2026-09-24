using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.Auth.Order;

/// <summary>
/// Response DTO containing the created order identifier.
/// </summary>
public class OrderResponse()
{
    /// <summary>
    /// Initializes a new instance of the <see cref="OrderResponse"/> class with the specified order identifier.
    /// </summary>
    /// <param name="orderId">The unique identifier of the created order.</param>
    public OrderResponse(Guid orderId)
        : this()
    {
        OrderId = orderId;
    }

    /// <summary>
    /// Gets or sets the unique identifier of the order.
    /// </summary>
    [Required]
    public Guid OrderId { get; set; }
}
