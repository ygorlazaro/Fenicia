using Fenicia.Auth.Enums;

namespace Fenicia.Auth.Domains.Order;

/// <summary>
/// Response model containing order information
/// </summary>
public class OrderResponse
{
    /// <summary>
    /// The unique identifier of the order
    /// </summary>
    /// <example>550e8400-e29b-41d4-a716-446655440000</example>
    public Guid Id { get; set; }

    /// <summary>
    /// The date when the order was created
    /// </summary>
    /// <example>2025-06-03T10:00:00Z</example>
    public DateTime SaleDate { get; set; }

    /// <summary>
    /// The current status of the order
    /// </summary>
    public OrderStatus Status { get; set; }

    /// <summary>
    /// The total amount of the order
    /// </summary>
    /// <example>299.99</example>
    public decimal TotalAmount { get; set; }
}
