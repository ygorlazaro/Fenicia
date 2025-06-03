using System.ComponentModel.DataAnnotations;

namespace Fenicia.Auth.Requests;

/// <summary>
/// Request model for creating a new order
/// </summary>
public class NewOrderRequest
{
    /// <summary>
    /// List of module details included in the order
    /// </summary>
    [Required]
    public List<NewOrderDetailRequest> Details { get; set; } = null!;
}