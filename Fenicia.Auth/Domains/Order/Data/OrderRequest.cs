using System.ComponentModel.DataAnnotations;
using Fenicia.Auth.Domains.OrderDetail.Data;

namespace Fenicia.Auth.Domains.Order.Data;

/// <summary>
/// Represents a request model for creating a new order in the system.
/// </summary>
/// <remarks>
/// This class encapsulates all necessary information required to create a new order,
/// including the list of order details containing module information.
/// </remarks>
public class OrderRequest
{
    /// <summary>
    /// Gets or sets the collection of order details included in the order.
    /// </summary>
    /// <remarks>
    /// Each detail represents a module that is being ordered.
    /// The collection cannot be null and must contain at least one item.
    /// </remarks>
    [Required(ErrorMessage = "Order details are required")]
    [MinLength(1, ErrorMessage = "At least one order detail is required")]
    public IEnumerable<OrderDetailRequest> Details { get; set; } = null!;
}
