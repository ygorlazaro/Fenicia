using System.ComponentModel.DataAnnotations;

using Fenicia.Auth.Domains.OrderDetail;

namespace Fenicia.Auth.Domains.Order;

/// <summary>
/// Request model for creating a new order
/// </summary>
public class OrderRequest
{
    /// <summary>
    /// List of module details included in the order
    /// </summary>
    [Required]
    public IEnumerable<OrderDetailRequest> Details { get; set; } = null!;
}
