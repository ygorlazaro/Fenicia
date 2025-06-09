using System.ComponentModel.DataAnnotations;

namespace Fenicia.Auth.Requests;

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
