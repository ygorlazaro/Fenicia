using System.ComponentModel.DataAnnotations;
using Fenicia.Auth.Enums;

namespace Fenicia.Auth.Domains.Order.Data;

/// <summary>
/// Represents a response model containing order information and details
/// </summary>
/// <remarks>
/// This class is used to transfer order data from the service layer to the client
/// </remarks>
public class OrderResponse
{
    /// <summary>
    /// Gets or sets the unique identifier of the order
    /// </summary>
    /// <example>550e8400-e29b-41d4-a716-446655440000</example>
    [Required]
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the date when the order was created
    /// </summary>
    /// <example>2025-06-03T10:00:00Z</example>
    [Required]
    [DataType(DataType.DateTime)]
    public DateTime SaleDate { get; set; }

    /// <summary>
    /// Gets or sets the current status of the order
    /// </summary>
    /// <remarks>
    /// Represents the current state of the order in its lifecycle
    /// </remarks>
    [Required]
    [EnumDataType(typeof(OrderStatus))]
    public OrderStatus Status { get; set; }

    /// <summary>
    /// Gets or sets the total monetary amount of the order
    /// </summary>
    /// <example>299.99</example>
    [Required]
    [Range(0, double.MaxValue)]
    [DataType(DataType.Currency)]
    public decimal TotalAmount { get; set; }
}
