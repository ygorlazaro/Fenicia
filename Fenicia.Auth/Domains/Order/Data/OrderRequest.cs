using Fenicia.Auth.Domains.OrderDetail.Data;

namespace Fenicia.Auth.Domains.Order.Data;

/// <summary>
/// Request model for creating a new order
/// </summary>
public class OrderRequest
{
    /// <summary>
    /// List of module details included in the order
    /// </summary>
    public IEnumerable<OrderDetailRequest> Details { get; set; } = null!;
}
